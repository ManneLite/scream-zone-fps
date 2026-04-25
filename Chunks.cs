using Godot;
using System;

public partial class Chunks : Node3D
{
	// Chunk
	[Export] public PackedScene ChunkInstance;
	
	[Export] public int ChunkWidth = 256;
	[Export] public int ChunkHeight = 256;
	[Export] public float ChunkHeightMultiplier = 10f;
	[Export] public float ChunkCellSize = 1f;
	
	// Noise
	[Export] public float NoiseFrequency = 0.01f;
	[Export] public int NoiseOctaves = 4;
	[Export] public float NoiseGain = 0.5f;
	[Export] public float NoiseLacunarity = 2.0f;
	
	public FastNoiseLite Noise;
	public int Seed;
	
	// Called when the node enters the scene tree for the first time.
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
		
		loadChunk(0,0);
		loadChunk(1,0);
		loadChunk(1,1);
		loadChunk(0,1);
	}
	
	public void loadChunk(int x,int y)
	{
        int offset_x = (x * ChunkWidth)-(x);
        int offset_z = (y * ChunkHeight)-(y);
		if(ChunkInstance.Instantiate() is chunk_mesh_3d chunk)
		{
			chunk.Width = ChunkWidth;
			chunk.Height = ChunkHeight;
			chunk.HeightMultiplier = ChunkHeightMultiplier;
			chunk.CellSize = ChunkCellSize;
			chunk.OffsetX = offset_x;
			chunk.OffsetZ = offset_z;
			chunk.Noise = Noise;
			
			AddChild(chunk);
			GD.Print(chunk.GlobalPosition);
		} else {
			GD.Print("Not a chunk");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
