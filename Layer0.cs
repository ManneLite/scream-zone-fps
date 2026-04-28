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

    RandomNumberGenerator rng = new();
	NavigationRegion3D nav_region;

	public int Seed = 0;

	private Dictionary<Vector2I, ChunkMesh3D> chunks = new();
	private ChunkCheckRay3D Raycast;
	private Player3D player;

	bool active = true;

	public override void _Ready()
	{
		nav_region = GetNode<NavigationRegion3D>("NavigationRegion3D");

		if(Seed == 0)
		{
			var rng = new RandomNumberGenerator();
			Seed = (int)rng.Randi();
		}
		GlobalNoise.Instance.SeedSet(Seed);
		GlobalNoise.Instance.SetChunkSize(ChunkSize);
		GlobalNoise.Instance.SetChunkHeight(ChunkHeight);

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
		bool should_rebake = false;

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
			chunks.Remove(r);
		}

		foreach (var pos in desired)
		{
			if (!chunks.ContainsKey(pos))
			{
				LoadChunk(pos);
				should_rebake = true;
			}
		}

		if(should_rebake)
		{
			nav_region.BakeNavigationMesh(true);
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

			chunk.MaterialOverride = GlobalSinInfo.Instance.GetShaderBySin(chunk.Sin);
			nav_region.AddChild(chunk);
			chunks[new Vector2I(x, z)] = chunk;
			
            rng.Seed = (uint)(("x"+x+"z"+z).Hash() + Seed);
            for(int i = 0; i < MaxObjectsPerChunk; ++i)
            {

				int object_type =  (int)(rng.Randi() % WorldObjects.Length);
                int global_chunk_pos_x = x * ChunkSize;
                int global_chunk_pos_z = z * ChunkSize;
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
