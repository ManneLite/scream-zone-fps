using Godot;
using System;

public partial class navigation_region_3d : NavigationRegion3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        GetNode<Chunks>("Chunks").ChunksLoaded += _on_chunks_chunks_loaded;
	}

    public void _on_chunks_chunks_loaded()
    {
		GD.Print("Baking Navigation Mesh");
		var start = Time.GetTicksUsec();
		BakeNavigationMesh(true);
		var end = Time.GetTicksUsec();
		GD.Print("Finished baking Navigation Mesh, it took " + ((float)(end - start) / 1000000f));
		GD.Print("Seed:", GetNode<Chunks>("Chunks").Seed);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
