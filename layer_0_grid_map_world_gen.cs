using Godot;
using System;

public partial class layer_0_grid_map_world_gen : GridMap
{
	[Export] public int WorldHeight = 5;
	[Export] public int WorldRadius = 25;
	[Export] public float Noise_Freq = 0.05f;

	FastNoiseLite noise = new();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		noise.Seed = (int)GD.Randi();
		noise.Frequency = Noise_Freq;
		noise.FractalType = FastNoiseLite.FractalTypeEnum.Fbm;
		noise.FractalOctaves = 3;
		noise.FractalLacunarity = 2.0f;
		noise.FractalGain = 0.5f;

        var start = Time.GetTicksUsec();
		GD.Print("Beginning world generation");
		for(int x = -WorldRadius; x < WorldRadius; ++x)
		{
			for(int z = -WorldRadius; z < WorldRadius; ++z)
			{
				float noise_value = noise.GetNoise3D(x, 0, z);
				int y = (int)(noise_value * WorldHeight);
				if(y <= (int)(-(float)WorldHeight*0.5f))
				{

					SetCellItem(new(x, y , z), 0);
					SetCellItem(new(x, y-1 , z), 0);
				}
				else
				{
					SetCellItem(new(x, y , z), 1);
					SetCellItem(new(x, y-1 , z), 1);
				}
			}
		}
        var end = Time.GetTicksUsec();
		GD.Print("World generation complete, It took " + ((float)(end - start)/1000000f));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
