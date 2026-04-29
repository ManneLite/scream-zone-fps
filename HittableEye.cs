using Godot;
using System;

public partial class HittableEye : Area3D
{
    Timer timer;

    [Export] PackedScene ProjectileInstance;
    [Export] public bool activated = false;
    [Export] public bool damaged = false;
        

	public override void _Ready()
	{
        timer = GetNode<Timer>("Timer");
        timer.Timeout += OnTimeout;
        if(activated)
        {
            timer.Start();
        }

	}

    public void OnTimeout()
    {
        if(activated && !damaged)
        {
            if(GetTree().GetFirstNodeInGroup("Player") is Node3D player)
            {
                if(ProjectileInstance.Instantiate() is Projectile3D projectile)
                {
                    projectile.Speed = 50f;
                    GetTree().CurrentScene.AddChild(projectile);
                    projectile.GlobalPosition = GlobalPosition;
                    projectile.LookAt(player.GlobalPosition);
                }
            }
        }
    }

    public void Activate()
    {
        if(!activated)
        {
            activated = true;
        }
    }

    public void Disable()
    {
        damaged = true;
    }

}
