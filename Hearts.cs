using Godot;
using System;

public partial class Hearts : CanvasLayer
{
	
	public Godot.Collections.Array<Godot.Node> HeartObjects;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		HeartObjects = GetTree().GetNodesInGroup("Hearts");
		foreach (Node Heart in HeartObjects)
		{
			GD.Print(Heart.Name);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
