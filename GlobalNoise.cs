using Godot;
using System;

public partial class GlobalNoise : Node
{
	public static GlobalNoise Instance {get; private set;}

	[Export] public float NoiseFrequency = 0.01f;
	[Export] public int NoiseOctaves = 4;
	[Export] public float NoiseGain = 0.5f;
	[Export] public float NoiseLacunarity = 2.0f;

	public int ChunkSize = 40;
	public float ChunkHeight = 30f;

	public FastNoiseLite Noise;

	public override void _Ready()
	{	
		Noise = new FastNoiseLite();
		Noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
		Noise.Frequency = NoiseFrequency;
		Noise.FractalOctaves = NoiseOctaves;
		Noise.FractalGain = NoiseGain;
		Noise.FractalLacunarity = NoiseLacunarity;
		Instance = this;
	}

	public void SeedSet(int seed_new)
	{
		Noise.Seed = seed_new;
	}

	public void SetChunkSize(int chunk_size)
	{
		ChunkSize = chunk_size;
	}

	public void SetChunkHeight(float chunk_height)
	{
		ChunkHeight = chunk_height;
	}

	public float GetYAtPosV3(Vector3 pos)
	{
		return GetYAtPos(pos.X, pos.Z);
	}

	public float GetYAtPosV2(Vector2 pos)
	{
		return GetYAtPos(pos.X, pos.Y);
	}

	public float GetYAtPos(float pos_x, float pos_y)
	{
		float result = Noise.GetNoise2D(pos_x, pos_y);
		result = (result + 1f) * 0.5f;
		result = (result * ChunkHeight);
		return result;
	}

}
