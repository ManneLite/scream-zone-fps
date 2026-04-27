using Godot;
using System.Collections.Generic;

public partial class Layer0 : Node3D
{
    NavigationRegion3D nav_region;
	[Export] public PackedScene ChunkTemplate;
    [Export] public PackedScene PlayerTemplate;

	[Export] public int ChunkSize = 40;
	[Export] public float ChunkHeight = 30f;

	[Export] public float NoiseFrequency = 0.01f;
	[Export] public int NoiseOctaves = 4;
	[Export] public float NoiseGain = 0.5f;
	[Export] public float NoiseLacunarity = 2.0f;

	public FastNoiseLite Noise;
	public int Seed = 0;

	private Dictionary<Vector2I, ChunkMesh3D> chunks = new();
	private ChunkCheckRay3D Raycast;
    private Player3D player;

	public BiomeMaterialMap3D BiomeShaders;

    public override void _Ready()
    {
        nav_region = GetNode<NavigationRegion3D>("NavigationRegion3D");

		Noise = new FastNoiseLite();
		Noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
		Noise.Frequency = NoiseFrequency;
		Noise.FractalOctaves = NoiseOctaves;
		Noise.FractalGain = NoiseGain;
		Noise.FractalLacunarity = NoiseLacunarity;

        if(Seed == 0)
        {
		    var rng = new RandomNumberGenerator();
		    Seed = (int)rng.Randi();
        }
		Noise.Seed = Seed;

		//Raycast.PlayerChangedChunk += OnPlayerChangedChunk;

		BiomeShaders = GetNode<BiomeMaterialMap3D>("BiomeMaterialMap3D");
		Load3x3Chunks(0, 0);

        if(PlayerTemplate.Instantiate() is Player3D p)
        {
            player = p;
            p.GetNode<ChunkCheckRay3D>("ChunkCheckRay3D").PlayerChangedChunk += OnPlayerChangedChunk;
            float n = Noise.GetNoise2D(0,0);
            n = (n + 1f) * 0.5f;
            float y = (n * ChunkHeight + 1);
            AddChild(player);
            player.GlobalPosition = new(0, y, 0);
            GD.Print(player.GlobalPosition);
        }
    }

	private float GetBiomeValue(int x, int z)
	{
		float scale = 0.02f;
		float n1 = Noise.GetNoise2D(x * scale, z * scale);
		float n2 = Noise.GetNoise2D((x + 10000) * scale, (z - 10000) * scale);
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
			chunk.Height = ChunkHeight;
			chunk.Pos = pos;
			chunk.Noise = Noise;

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
			    GD.Print(a);
			    chunk.MaterialOverride = BiomeShaders.biomeShaders[a];
            }

			nav_region.AddChild(chunk);
			chunks[new Vector2I(x, z)] = chunk;
		}
	}
	private void OnPlayerChangedChunk(Vector2 local)
	{
		Load3x3Chunks((int)local.X, (int)local.Y);
	}
}
