using Godot;
using System;

public partial class layer_0_grid_map_world_gen : GridMap
{
	[Export] public int WorldHeight = 5;
	[Export] public int WorldRadius = 25;

	FastNoiseLite noise = new();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		noise.Seed = (int)GD.Randi();
		noise.Frequency = 0.05f;

		for(int x = -WorldRadius; x < WorldRadius; ++x)
		{
			for(int z = -WorldRadius; z < WorldRadius; ++z)
			{
				for(int y = 0; y < WorldHeight; ++y)
				{
					float noise_value = noise.GetNoise3D(x, y, z);
					int type_id = 2;
					if(!(noise_value < -0.1))
					{
						type_id = noise_value >= 0 ? 0 : 1;
					}

					if(type_id != 2)
					{
						SetCellItem(new(x, y , z), type_id);
					}
				}
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
