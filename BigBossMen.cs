using Godot;
using System;

public partial class BigBossMen : MeshInstance3D
{
	[Signal] public delegate void BossDiedEventHandler();

	[Export]
	public Vector3 RotationSpeed = new Vector3(0, 0.1f, 0);

    Timer timer;

    [Export] bool active;
    int eyes_active = 0;
    bool voulnurable = false;
    Godot.Collections.Array<HittableEye> eyes = new();

    override public void _Ready()
    {

        timer = GetNode<Timer>("Timer");
        foreach(var child in GetChildren())
        {
            if(child is HittableEye eye)
            {
                eyes.Add(eye);
            }
        }
        if(active)
        {
            Activate();
        }
    }

    public void Activate()
    {
        active = true;

        eyes.Shuffle();
        
        timer.Start();
    }

    public void OpenMiddleEye()
    {
        voulnurable = true;
    }

    public void OnEyeHit()
    {
        if(--eyes_active <= 0)
        {
            GD.Print(eyes_active);
            GD.Print("activating");
            Activate();
        }
    }

    public void OnTimerTimeout()
    {
        if(eyes_active < 10)
        {
            if(eyes.Count > 0)
            {
                eyes[0].Activate();
                eyes[0].EyeGotHit += OnEyeHit;
                eyes.RemoveAt(0);

                ++eyes_active;
            }
            else
            {
				EmitSignal(SignalName.BossDied);
            }
        }
        else
        {
            active = false;
            timer.Stop();
        }
    }

	public override void _Process(double delta)
	{
		RotateObjectLocal(Vector3.Right, RotationSpeed.X * (float)delta);
		RotateObjectLocal(Vector3.Up, RotationSpeed.Y * (float)delta);
		RotateObjectLocal(Vector3.Forward, RotationSpeed.Z * (float)delta);
	}
}
