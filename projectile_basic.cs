using Godot;
using System;

public partial class projectile_basic : Area3D
{
	[Export] public float Speed = 750f;
	public Vector3 Direction;
	bool active = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var particle_system = GetNode<GpuParticles3D>("GPUParticles3D");
		var mat = particle_system.ProcessMaterial as ParticleProcessMaterial;
		if (mat != null)
		{
			mat.Gravity = Direction.Normalized() * Speed/3.0f;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition += Direction * Speed * (float)delta;
	}

	private void _on_body_entered(Node3D body)
	{
		if(active)
		{
			active = false;
            if(body is IDamagable target)
            {
                target.take_damage();
            }
		}
		QueueFree();
	}

	private void _on_timer_timeout()
	{
		active = false;
		QueueFree();
	}
}
