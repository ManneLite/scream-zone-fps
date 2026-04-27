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

    NavigationRegion3D nav_region;

	public int Seed = 0;

	private Dictionary<Vector2I, ChunkMesh3D> chunks = new();
	private ChunkCheckRay3D Raycast;
    private Player3D player;

	public BiomeMaterialMap3D BiomeShaders;
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

		BiomeShaders = GetNode<BiomeMaterialMap3D>("BiomeMaterialMap3D");
		Load3x3Chunks(0, 0);

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

	private float GetBiomeValue(int x, int z)
	{
        GD.Print("Getting biome value");
		float scale = 0.02f;
		float n1 = GlobalNoise.Instance.Noise.GetNoise2D(x * scale, z * scale);
		float n2 = GlobalNoise.Instance.Noise.GetNoise2D((x + 10000) * scale, (z - 10000) * scale);
		float value = n1 * 0.5f + n2 * 0.5f;
		return (value + 1f) * 0.5f;
	}

	public void Load3x3Chunks(int x, int z)
	{
        bool should_rebake = false;

		HashSet<Vector2I> desired = new();

		for (int ax = -1; ax <= 1; ax++)
		{
			for (int az = -1; az <= 1; az++)
			{
				desired.Add(new Vector2I(x + ax, z + az));
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
			    chunk.MaterialOverride = BiomeShaders.biomeShaders[Vector2.Zero];
            }
            else
            {
			    Vector2 a = new Vector2(x,z).Normalized();
			    a = new Vector2(Mathf.Round(a.X), Mathf.Round(a.Y));
			    chunk.MaterialOverride = BiomeShaders.biomeShaders[a];
            }


			nav_region.AddChild(chunk);
			chunks[new Vector2I(x, z)] = chunk;

            if(EnemySpawnerTemplate.Instantiate() is EnemySpawner spawner)
            {
                Vector3 spawner_pos = new Vector3(pos.X, 0, pos.Y) * ChunkSize;
                spawner_pos.Y = GlobalNoise.Instance.GetYAtPosV3(spawner_pos) + 1;
                chunk.AddChild(spawner);
                spawner.GlobalPosition = spawner_pos;
            }
		}
	}
	private void OnPlayerChangedChunk(Vector2 local)
	{
        if(active)
        {
		    Load3x3Chunks((int)local.X, (int)local.Y);
        }
	}

    public void OnPlayerDied()
    {
        active = false;
        
        // fix this and handle cyclic dependencies
        GetTree().ChangeSceneToFile("res://MenuMain.tscn");
    }
}
