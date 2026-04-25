using Godot;
using System;
using System.Collections.Generic;

public partial class Chunks : Node3D
{
	[Export] public PackedScene ChunkInstance;

	[Export] public int ChunkWidth = 256;
	[Export] public int ChunkHeight = 256;
	[Export] public float ChunkHeightMultiplier = 10f;
	[Export] public float ChunkCellSize = 1f;

	[Export] public float NoiseFrequency = 0.01f;
	[Export] public int NoiseOctaves = 4;
	[Export] public float NoiseGain = 0.5f;
	[Export] public float NoiseLacunarity = 2.0f;

	public FastNoiseLite Noise;
	public int Seed;

	private Dictionary<Vector2I, chunk_mesh_3d> chunks = new();
	private Dictionary<int, StandardMaterial3D> biomeShaders = new();
	private RayCast3d Raycast;
	
	public Node3d BiomeShaders;

	public override void _Ready()
	{
		Noise = new FastNoiseLite();
		Noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
		Noise.Frequency = NoiseFrequency;
		Noise.FractalOctaves = NoiseOctaves;
		Noise.FractalGain = NoiseGain;
		Noise.FractalLacunarity = NoiseLacunarity;

		var rng = new RandomNumberGenerator();
		Seed = (int)rng.Randi();
		Noise.Seed = Seed;

		Raycast = GetTree().GetFirstNodeInGroup("PlayerRaycast") as RayCast3d;
		Raycast.PlayerChangedChunk += OnPlayerChangedChunk;
		
		BiomeShaders = GetNode<Node3d>("BiomeShaders");
		biomeShaders[0] = BiomeShaders.Greed;
		biomeShaders[1] = BiomeShaders.Pride;
		biomeShaders[2] = BiomeShaders.Sloth;
		biomeShaders[3] = BiomeShaders.Wrath;
		biomeShaders[4] = BiomeShaders.Gluttony;
		biomeShaders[5] = BiomeShaders.Envy;
		biomeShaders[6] = BiomeShaders.Lust;

		Load3x3Chunks(0, 0);
	}

	private void OnPlayerChangedChunk(Vector2 local)
	{
		Load3x3Chunks((int)local.X, (int)local.Y);
	}

	public void Load3x3Chunks(int x, int z)
	{
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
				LoadChunk(pos.X, pos.Y);
			}
		}
	}

	public void LoadChunk(int x, int z)
	{
		int offsetX = (x * ChunkWidth) - x;
		int offsetZ = (z * ChunkHeight) - z;

		if (ChunkInstance.Instantiate() is chunk_mesh_3d chunk)
		{
			chunk.Width = ChunkWidth;
			chunk.Height = ChunkHeight;
			chunk.HeightMultiplier = ChunkHeightMultiplier;
			chunk.CellSize = ChunkCellSize;
			chunk.OffsetX = offsetX;
			chunk.OffsetZ = offsetZ;
			chunk.LocalPosX = x;
			chunk.LocalPosZ = z;
			chunk.Noise = Noise;
			
			float noiseValue = Noise.GetNoise2D(x, z);

			float normalized = (noiseValue + 1f) * 0.5f;

			int step = (int)MathF.Floor(normalized * 7f);

			step = Mathf.Clamp(step, 0, 6);

			chunk.MaterialOverride = biomeShaders[step];

			AddChild(chunk);
			chunks[new Vector2I(x, z)] = chunk;
		}
	}
}
