using Godot;
using System;
using System.Collections.Generic;

public partial class Node3d : Node3D
{
	[Export] public StandardMaterial3D Greed;
	[Export] public StandardMaterial3D Pride;
	[Export] public StandardMaterial3D Sloth;
	[Export] public StandardMaterial3D Wrath;
	[Export] public StandardMaterial3D Gluttony;
	[Export] public StandardMaterial3D Envy;
	[Export] public StandardMaterial3D Lust;
	[Export] public StandardMaterial3D EndGame;
	[Export] public StandardMaterial3D EndGameActivated;
	
	public Dictionary<Vector2, StandardMaterial3D> biomeShaders = new();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		biomeShaders[new Vector2(0,0)] = EndGameActivated;
		biomeShaders[new Vector2(1,0)] = EndGame;
		biomeShaders[new Vector2(-1,0)] = Lust;
		biomeShaders[new Vector2(0,1)] = Envy;
		biomeShaders[new Vector2(0,-1)] = Gluttony;
		biomeShaders[new Vector2(1,1)] = Wrath;
		biomeShaders[new Vector2(-1,-1)] = Sloth;
		biomeShaders[new Vector2(-1,1)] = Pride;
		biomeShaders[new Vector2(1,-1)] = Greed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
