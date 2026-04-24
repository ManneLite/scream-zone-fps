using Godot;
using System;

public partial class player_controller : CharacterBody3D
{
	[Export] public float Base_Speed = 5.0f;
	[Export] public float JumpVelocity = 4.5f;
	[Export] public float Sensitivity = 0.1f;
	Node3D Head;

	public override void _Ready()
	{
		Input.MouseMode = Godot.Input.MouseModeEnum.Captured;
		Head = GetNode<Node3D>("Head3D");
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if(@event is InputEventMouseMotion motion)
		{
			RotateY(-motion.Relative.X * Sensitivity);
			float head_rotation = -motion.Relative.Y * Sensitivity;
			float rotation_new = Head.Rotation.X + head_rotation;
			if(rotation_new <= 1.5f && rotation_new >= -1.5f)
			{
				Head.RotateX(head_rotation); // input here is relative
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		float speed = Base_Speed;


		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("Move_Jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		if (Input.IsActionPressed("Move_Sprint"))
		{
			speed *= 1.5f;
		}

		// Get the input direction and handle the movement/deceleration.
		Vector2 inputDir = Input.GetVector("Move_Left", "Move_Right", "Move_Forward", "Move_Backward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * speed;
			velocity.Z = direction.Z * speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
