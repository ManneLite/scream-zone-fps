using Godot;
using System;

public partial class navigation_region_3d : NavigationRegion3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Baking Navigation Mesh");
		var start = Time.GetTicksUsec();
		BakeNavigationMesh(true);
		var end = Time.GetTicksUsec();
		GD.Print("Finished baking Navigation Mesh, it took " + ((float)(end - start) / 1000000f));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
