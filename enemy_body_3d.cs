using Godot;
using System;

public partial class enemy_body_3d : CharacterBody3D, IDamagable
{
	[Export] public float Speed = 5.0f;
	[Export] public CharacterBody3D Target;
	[Export] public int hp = 1;
	bool active = true;

	NavigationAgent3D nav_agent;

	public override void _Ready()
	{
		nav_agent = GetNode<NavigationAgent3D>("NavigationAgent3D");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

        if(Target is not null)
        {
            Vector3 target_pos = Target.GlobalPosition;
		    nav_agent.SetTargetPosition(target_pos);
		    Vector3 nav_point_next = nav_agent.GetNextPathPosition();

            LookAt(nav_point_next with {Y = GlobalPosition.Y}, Vector3.Up);

            velocity = (nav_point_next - GlobalPosition).Normalized() * Speed;
		    Velocity = velocity;
		    MoveAndSlide();
        }
	}

	public void take_damage()
	{
        --hp;
        GD.Print("Took damage, hp left: " + hp);
		if(hp <= 0 && active)
		{
			active = false;
			
			// play death animation here

			QueueFree();
		}
	}
}
