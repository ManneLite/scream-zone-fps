using Godot;
using System;

public partial class Node3d : Node3D
{
	[Export] public StandardMaterial3D Greed;
	[Export] public StandardMaterial3D Pride;
	[Export] public StandardMaterial3D Sloth;
	[Export] public StandardMaterial3D Wrath;
	[Export] public StandardMaterial3D Gluttony;
	[Export] public StandardMaterial3D Envy;
	[Export] public StandardMaterial3D Lust;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
