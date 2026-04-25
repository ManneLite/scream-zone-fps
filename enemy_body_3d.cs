using Godot;
using System;

public partial class enemy_body_3d : CharacterBody3D, IDamagable
{
	[Export] public float Speed = 5.0f;
	[Export] public NodePath PlayerPath;
	[Export] public int hp = 1;
	bool active = true;

	CharacterBody3D player;
	NavigationAgent3D nav_agent;

	public override void _Ready()
	{
		player = GetNode<CharacterBody3D>(PlayerPath);
		nav_agent = GetNode<NavigationAgent3D>("NavigationAgent3D");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		nav_agent.SetTargetPosition(player.GlobalPosition);
		Vector3 nav_point_next = nav_agent.GetNextPathPosition();
		velocity = (nav_point_next - GlobalPosition).Normalized() * Speed;

		Velocity = velocity;
		MoveAndSlide();
	}

	public void take_damage()
	{
		if(--hp <= 0 && active)
		{
			active = false;
			
			// play death animation here

			QueueFree();
		}
	}
}
