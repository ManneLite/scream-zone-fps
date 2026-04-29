using Godot;
using System;

public partial class Player3D : CharacterBody3D, IDamagable
{
	[Signal] public delegate void PlayerHPChangedEventHandler(int current_hp);
	[Signal] public delegate void PlayerDiedEventHandler();
	[Signal] public delegate void SendPlayerPositionEventHandler(Vector3 pos);
	[Signal] public delegate void PlayerChangedChunksEventHandler(string sin);

	[Export] public float Base_Speed = 5.0f;
	[Export] public float JumpVelocity = 4.5f;
	[Export] public float Sensitivity = 0.1f;
	[Export] public PackedScene Projectile;
	[Export] public int HP = 10;
	public AudioStreamPlayer2D shoot_sfx;
	bool alive = true;
	Node3D Head;
	Node3D ProjectileSpawnPos;
	RayCast3D ProjectileRay;

	Node3D ProjectileMesh;

	Area3D EnemyDetectionSphere;

	SinType CurrentSin = SinType.Divine;

	public override void _Ready()
	{
		Input.MouseMode = Godot.Input.MouseModeEnum.Captured;
		Head = GetNode<Node3D>("Head3D");
		ProjectileMesh = Head.GetNode<Node3D>("ProjectileMesh");
		ProjectileSpawnPos = Head.GetNode<Node3D>("ProjectileSpawnPosition3D");
		ProjectileRay = Head.GetNode<RayCast3D>("ProjectileRay3D");
		shoot_sfx = GetNode<AudioStreamPlayer2D>("SFX_Shoot");
		EnemyDetectionSphere = GetNode<Area3D>("EnemyDetectionSphere");
		Timer send_position_timer = GetNode<Timer>("SendPositionTimer");
		send_position_timer.Timeout += OnSendPositionTimerTimeout;
		EnemyDetectionSphere.BodyEntered += OnEnemyEnteredDetectionRange;
		EnemyDetectionSphere.BodyExited += OnEnemyLeftDetectionRange;
		GetNode<ChunkCheckRay3D>("ChunkCheckRay3D").PlayerChangedChunk += OnChunkChanged;

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

		if(Input.IsActionJustPressed("Action_Attack"))
		{
			shoot();
		}
	}

	public void shoot()
	{
			Vector3 target;
			if(ProjectileRay.IsColliding())
			{
				target = ProjectileRay.GetCollisionPoint();
				if(ProjectileRay.GetCollider() is IDamagable enemy)
				{
					enemy.take_damage();
				}
			}
			else
			{
				target = Head.GlobalPosition + (Head.GlobalTransform.Basis.Z * ProjectileRay.TargetPosition.Z);
			}

			shoot_sfx.Play(0.5f);
			Vector3 scale_new = new(0.05f, 0.05f ,ProjectileMesh.GlobalPosition.DistanceTo(target));
			ProjectileMesh.Scale = scale_new;
			ProjectileMesh.LookAt(target);
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		float speed = Base_Speed;

		if(ProjectileMesh.Scale.X < 0.03)
		{
			ProjectileMesh.Scale = Vector3.Zero;
		}
		else if(ProjectileMesh.Scale.X != 0)
		{
			float z_scale = ProjectileMesh.Scale.Z;
			ProjectileMesh.Scale = ProjectileMesh.Scale.Lerp(Vector3.Zero, (float)delta*5) with {Z = z_scale};
		}

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		/*
		if (Input.IsActionJustPressed("Move_Jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}
		*/
		if (Input.IsActionJustPressed("Move_Jump"))
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

	public void take_damage()
	{
		if(alive)
		{
			alive = --HP > 0;
			EmitSignal(SignalName.PlayerHPChanged, HP);
			if(!alive)
			{
				GD.Print("You Died");
				Input.MouseMode = Godot.Input.MouseModeEnum.Visible;
				EmitSignal(SignalName.PlayerDied);
			}
		}
	}

	public void OnEnemyEnteredDetectionRange(Node3D body)
	{
		if(body is EnemyBody3D enemy)
		{
			SendPlayerPosition += enemy.SetTarget;
		}
	}

	public void OnEnemyLeftDetectionRange(Node3D body)
	{
		if(body is EnemyBody3D enemy)
		{
			SendPlayerPosition -= enemy.SetTarget;
		}
	}

	public void OnSendPositionTimerTimeout()
	{
		EmitSignal(SignalName.SendPlayerPosition, GlobalPosition);
	}

	public void OnChunkChanged(Vector2I ChunkPos, SinType ChunkSin)
	{
		if(CurrentSin != ChunkSin)
		{
			CurrentSin = ChunkSin;
			EmitSignal(SignalName.PlayerChangedChunks, ChunkSin.ToString());
		}
	}

}
