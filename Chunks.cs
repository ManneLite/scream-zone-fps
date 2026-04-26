using Godot;
using System;
using System.Collections.Generic;

public partial class Chunks : Node3D
{
	[Signal] public delegate void ChunksLoadedEventHandler();

	[Export] public PackedScene ChunkInstance;

	[Export] public int ChunkSize = 40;
	[Export] public float ChunkHeight = 30f;

	[Export] public float NoiseFrequency = 0.01f;
	[Export] public int NoiseOctaves = 4;
	[Export] public float NoiseGain = 0.5f;
	[Export] public float NoiseLacunarity = 2.0f;

	public FastNoiseLite Noise;
	public int Seed;

	private Dictionary<Vector2I, chunk_mesh_3d> chunks = new();
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

		Load3x3Chunks(0, 0);
	}

	private void OnPlayerChangedChunk(Vector2 local)
	{
		Load3x3Chunks((int)local.X, (int)local.Y);
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
				GD.Print("setting bake to true");
				should_rebake = true;
			}
		}

		if(should_rebake)
		{
			GD.Print("sending bake signal");
			EmitSignal(SignalName.ChunksLoaded);
		}
	}

	public void LoadChunk(Vector2I pos)
	{
		if (ChunkInstance.Instantiate() is chunk_mesh_3d chunk)
		{
			chunk.Size = ChunkSize;
			chunk.Height = ChunkHeight;
			chunk.Pos = pos;
			chunk.Noise = Noise;

			int x = pos.X;
			int z = pos.Y;

			if(x == 0 && z == 0)
			{
				chunk.MaterialOverride = BiomeShaders.biomeShaders[new(1,1)];
			}
			else if(-1 <= x && 1 >= x && -1 <= z && 1 >= z)
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
			


			AddChild(chunk);
			chunks[new Vector2I(x, z)] = chunk;
		}
	}
}
