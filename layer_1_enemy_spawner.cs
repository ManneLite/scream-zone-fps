using Godot;
using System;

public partial class layer_1_enemy_spawner : Node3D
{
	[Export] public PackedScene Enemy;
	[Export] public CharacterBody3D Player;

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_timer_timeout()
	{
		if(Enemy.Instantiate() is EnemyBody3D enemy)
		{
			float pos_x = (float)GD.RandRange(-40f, 40f);
			float pos_z = (float)GD.RandRange(-40f, 40f);
			enemy.Target = Player;
			AddChild(enemy);
			enemy.GlobalPosition = new(pos_x, 60, pos_z);
			enemy.SmoothRotation = true;
			enemy.HP = 3;
		}
	}
}
