using Godot;
using System;

public partial class Projectile3D : Area3D
{
	[Export] public float Speed = 750f;
	bool active = true;
    Timer timer;
    AudioStreamPlayer3D sfx_exploding;
    GpuParticles3D particle_system;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        timer = GetNode<Timer>("Timer");
		sfx_exploding = GetNode<AudioStreamPlayer3D>("ExplodingSFX");
		particle_system = GetNode<GpuParticles3D>("GPUParticles3D");
		var mat = particle_system.ProcessMaterial as ParticleProcessMaterial;
		if (mat != null)
		{
			mat.Gravity = -Transform.Basis.Z * Speed/3.0f;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition += -Transform.Basis.Z * Speed * (float)delta * (active ? 1 : 0);
	}

	private void _on_body_entered(Node3D body)
	{
		if(active)
		{
            timer.Start();
			active = false;
			if(body is IDamagable target)
			{
				target.take_damage();
                particle_system.Visible = false;
			}
            sfx_exploding.Play();
		}
	}

	private void _on_timer_timeout()
	{
		active = false;
		QueueFree();
	}
}
