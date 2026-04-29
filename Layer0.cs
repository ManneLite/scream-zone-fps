using Godot;
using System.Collections.Generic;

public partial class Layer0 : Node3D
{
	//[Export] public PackedScene PlayerDiedScene;
	[Export] public PackedScene ChunkTemplate;
	[Export] public PackedScene PlayerTemplate;
	[Export] public PackedScene EnemySpawnerTemplate;

	[Export] public int ChunkSize = 40;
	[Export] public float ChunkHeight = 30f;

	[Export] public PackedScene[] WorldObjects;
	[Export] public int MaxObjectsPerChunk = 15;

	public NavigationMeshSourceGeometryData3D navigation_geometry = new();

	RandomNumberGenerator rng = new();

	public int Seed = 0;

	private Dictionary<Vector2I, NavigationRegion3D> chunk_pos_to_region = new();
	private Dictionary<Vector2I, ChunkMesh3D> chunks = new();
	private ChunkCheckRay3D Raycast;
	private Player3D player;

	public Node3D ChunksRoot;

	bool active = true;

	public override void _Ready()
	{
		ChunksRoot = GetNode<Node3D>("ChunksRoot");
		if(Seed == 0)
		{
			var rng = new RandomNumberGenerator();
			Seed = (int)rng.Randi();
		}
		GlobalNoise.Instance.SeedSet(Seed);
		GlobalNoise.Instance.SetChunkSize(ChunkSize);
		GlobalNoise.Instance.SetChunkHeight(ChunkHeight);

		var map = GetWorld3D().NavigationMap;
		NavigationServer3D.MapSetCellSize(map, 0.25f);
		NavigationServer3D.MapSetUseEdgeConnections(map, true);

		Load3x3Chunks(new(0, 0));


		if(PlayerTemplate.Instantiate() is Player3D p)
		{

			p.PlayerDied += OnPlayerDied;
			p.GetNode<ChunkCheckRay3D>("ChunkCheckRay3D").PlayerChangedChunk += OnPlayerChangedChunk;
			float y = GlobalNoise.Instance.GetYAtPos(0, 0) + 1;
			player = p;
			AddChild(player);
			player.GlobalPosition = new(0, y, 0);
		}
	}

	public void Load3x3Chunks(Vector2I center_pos)
	{
		HashSet<Vector2I> desired = new();

		for (int ax = -1; ax <= 1; ax++)
		{
			for (int az = -1; az <= 1; az++)
			{
				desired.Add(new Vector2I(ax, az) + center_pos);
			}
		}

		List<Vector2I> toRemove = new();

		foreach (var kv in chunks)
		{
			if (!desired.Contains(kv.Key))
			{
				kv.Value.QueueFree();
				toRemove.Add(kv.Key);
			}
		}

		foreach (var r in toRemove)
		{
			if(chunk_pos_to_region.TryGetValue(r, out NavigationRegion3D region))
			{
				region.Enabled = false;
			}
			chunks.Remove(r);
		}

		foreach (var pos in desired)
		{
			if (!chunks.ContainsKey(pos))
			{
				LoadChunk(pos);
			}
		}
	}

	public void LoadChunk(Vector2I pos)
	{
		if (ChunkTemplate.Instantiate() is ChunkMesh3D chunk)
		{
			chunk.Size = ChunkSize;
			chunk.Pos = pos;

			int x = pos.X;
			int z = pos.Y;

			if(-1 <= x && 1 >= x && -1 <= z && 1 >= z)
			{
				chunk.Sin = SinType.Divine;
			}
			else
			{
				Vector2 a = new Vector2(x,z).Normalized();
				a = new Vector2(Mathf.Round(a.X), Mathf.Round(a.Y));
				chunk.Sin = GlobalSinInfo.Instance.GetSinByChunkPos(a);
			}

			int global_chunk_pos_x = x * ChunkSize;
			int global_chunk_pos_z = z * ChunkSize;
			if(!chunk_pos_to_region.ContainsKey(pos))
			{
				chunk.add_geometry_to_navigation_data = true;
				chunk.navigation_geometry = navigation_geometry;
			}

			chunk.MaterialOverride = GlobalSinInfo.Instance.GetShaderBySin(chunk.Sin);
			ChunksRoot.AddChild(chunk);
			chunks[pos] = chunk;

			if(chunk_pos_to_region.TryGetValue(pos, out NavigationRegion3D region))
			{
				region.Enabled = true;
			}
			else
			{
				NavigationMesh nav_mesh = new();
				nav_mesh.GeometryParsedGeometryType = NavigationMesh.ParsedGeometryType.StaticColliders;
				nav_mesh.FilterBakingAabb = new(
						new((global_chunk_pos_x - (ChunkSize/2)), 0, (global_chunk_pos_z - (ChunkSize/2))),
						new(ChunkSize, ChunkHeight, ChunkSize)
						);
				nav_mesh.FilterBakingAabb = nav_mesh.FilterBakingAabb.Grow(1);

				Callable callable = Callable.From(() => AddBakedRegion(pos, nav_mesh));
				NavigationRegion3D region_new = new();
				chunk_pos_to_region[pos] = region_new;

				NavigationServer3D.BakeFromSourceGeometryDataAsync(nav_mesh, navigation_geometry, callable);

			}
			
			rng.Seed = (uint)(("x"+x+"z"+z).Hash() + Seed);
			for(int i = 0; i < MaxObjectsPerChunk; ++i)
			{

				int object_type =  (int)(rng.Randi() % WorldObjects.Length);
				if(WorldObjects[object_type].Instantiate() is Node3D world_object)
				{
					float pos_x = (((rng.Randi() % ChunkSize) - (ChunkSize/2)) + 0.5f) + (x * ChunkSize);
					float pos_z = (((rng.Randi() % ChunkSize) - (ChunkSize/2)) + 0.5f) + (z * ChunkSize);
					float pos_y = GlobalNoise.Instance.GetYAtPos(pos_x, pos_z);
					chunk.AddChild(world_object);
					world_object.GlobalPosition = new(pos_x, pos_y, pos_z);
				}
			}

			if(EnemySpawnerTemplate.Instantiate() is EnemySpawner spawner)
			{
				Vector3 spawner_pos = new Vector3(pos.X, 0, pos.Y) * ChunkSize;
				spawner_pos.Y = GlobalNoise.Instance.GetYAtPosV3(spawner_pos) + 1;
				spawner.Sin = chunk.Sin;
				chunk.AddChild(spawner);
				spawner.GlobalPosition = spawner_pos;
			}

		}
	}

	public void AddBakedRegion(Vector2I pos, NavigationMesh nav_mesh)
	{
		NavigationRegion3D region = chunk_pos_to_region[pos];
		nav_mesh.FilterBakingAabb = new();
		var navmesh_vertices = nav_mesh.Vertices;
		GD.Print(nav_mesh.GetPolygonCount());

		GD.Print(navmesh_vertices.Length);
		for(int i = 0; i < navmesh_vertices.Length; ++i)
		{
			Vector3 vertex = navmesh_vertices[i];
			if(i == 0)
			{
				GD.Print(vertex);
				GD.Print(vertex.Snapped(1 * 0.1f));
			}
			navmesh_vertices[i] = vertex.Snapped(0.25f * 0.1f);
		}
		nav_mesh.Vertices = navmesh_vertices;

		region.NavigationMesh = nav_mesh;

		AddChild(region);
	}

	private void OnPlayerChangedChunk(Vector2I ChunkPos, SinType ChunkSin)
	{
		if(active)
		{
			Load3x3Chunks(ChunkPos);
		}
	}

	public void OnPlayerDied()
	{
		active = false;
		
		GetTree().ChangeSceneToFile("res://MenuMain.tscn");
	}
}
