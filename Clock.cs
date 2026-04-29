using Godot;
using System;
using System.Threading.Tasks;

public partial class Clock : Node
{
	public static Clock Instance;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}
	
	public void Wait(float WaitTime, Action FunctionToCallAfterTimeout)
	{
		var timer = new Timer();
		timer.OneShot = true;
		timer.WaitTime = WaitTime;
		timer.Timeout += () => {
			FunctionToCallAfterTimeout();
			timer.QueueFree();
		};
		
		AddChild(timer);
		timer.Start();
	} 
	
	public async Task AsyncWait(float WaitTime)
	{
		var timer = new Timer();
		timer.OneShot = true;
		timer.WaitTime = WaitTime ;
		
		AddChild(timer);
		timer.Start();
		
		await ToSignal(timer, Timer.SignalName.Timeout);
		timer.QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
