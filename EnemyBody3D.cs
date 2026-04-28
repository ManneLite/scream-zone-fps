using Godot;
using System;

public partial class EnemyBody3D : CharacterBody3D, IDamagable
{
	[Export] public float Speed = 5.0f;
	[Export] public float RotationSpeed = 5.0f;
	[Export] public int HP = 1;
	[Export] public bool SmoothRotation = false;
	public Sprite3D eye_open;
	public Sprite3D eye_half;
	public Sprite3D eye_closed;

	Timer attack_timer;
	bool attack_has_target = false;
	IDamagable attack_target;

	bool can_attack = true;
	bool active = true;
	float epsilon = 0.00001f;

	NavigationAgent3D nav_agent;

	public override void _Ready()
	{
		nav_agent = GetNode<NavigationAgent3D>("NavigationAgent3D");
		attack_timer = GetNode<Timer>("Timer");
		Node3D eyes = GetNode<Node3D>("Eyes");
		eye_open = eyes.GetNode<Sprite3D>("Sprite3D_Eye_Open");
		eye_half = eyes.GetNode<Sprite3D>("Sprite3D_Eye_Half");
		eye_closed = eyes.GetNode<Sprite3D>("Sprite3D_Eye_Closed");
	}

	public void SetTarget(Vector3 pos)
	{
		nav_agent.SetTargetPosition(pos);
	}

	public override void _PhysicsProcess(double delta)
	{
		if(!nav_agent.IsNavigationFinished())
		{
			Vector3 nav_point_next = nav_agent.GetNextPathPosition();
	
			Vector3 direction = GlobalPosition.DirectionTo(nav_point_next);
			Velocity = direction * Speed;
			Vector3 rotation = Rotation;
			rotation.Y = Mathf.LerpAngle(rotation.Y, Mathf.Atan2(-direction.X, -direction.Z), RotationSpeed * (float)delta);
			Rotation = rotation;
			/*
			Vector3 flat_target = new(global_pos.X + velocity.X, global_pos.Y, global_pos.Z + velocity.Z);
	
			if(!vec3_zero_approx(flat_target - global_pos))
			{
				LookAt(flat_target, Vector3.Up);
			}
			*/
			MoveAndSlide();
		}
	}

	public bool vec3_zero_approx(Vector3 vec)
	{
		return vec3_equal_approx(vec, Vector3.Zero);
	}

	public bool vec3_equal_approx(Vector3 a, Vector3 b)
	{
		bool equal_x_l = (a.X > (b.X - epsilon));
		bool equal_x_r = (a.X < (b.X + epsilon));
		bool equal_y_l = (a.Y > (b.Y - epsilon));
		bool equal_y_r = (a.Y < (b.Y + epsilon));
		bool equal_z_l = (a.Z > (b.Z - epsilon));
		bool equal_z_r = (a.Z < (b.Z + epsilon));

		bool result = equal_x_l && equal_x_r && equal_y_l && equal_y_r && equal_z_l && equal_z_r;
		return result;
	}

	public void take_damage()
	{
		--HP;
		GD.Print("Took damage, HP left: " + HP);
		if(HP == 2)
		{
			eye_open.Visible = false;
			eye_half.Visible = true;
		}
		else if(HP == 1)
		{
			eye_half.Visible = false;
			eye_closed.Visible = true;
		}
		
		if(HP <= 0 && active)
		{
			active = false;
			
			// play death animation here

			QueueFree();
		}
	}

	public void _on_attack_timer_timeout()
	{
		if(!can_attack)
		{
			can_attack = true;
		}
		else if(attack_has_target)
		{
			can_attack = false;
			attack_target.take_damage();
			attack_timer.Start();
		}
	}

	public void _on_body_exited_damage_box(Node3D body)
	{
		attack_has_target = false;
		attack_target = null;
	}

	public void _on_body_entered_damage_box(Node3D body)
	{
		if(body is IDamagable target)
		{
			attack_target = target;
			attack_has_target = true;
		}

		if(can_attack && attack_has_target)
		{
			can_attack = false;
			attack_target.take_damage();
			attack_timer.Start();
		}
	}

}
