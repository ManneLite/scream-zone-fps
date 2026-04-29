using Godot;
using System;
using System.Threading.Tasks;

public partial class Easing : Node
{
	
	public static Easing Instance;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}
	
	public async Task AsyncTween(GodotObject ObjectToTween, string Property, Variant Goal, float TweenTime)
	{
		var tween = CreateTween();
		
		tween.TweenProperty(ObjectToTween, Property, Goal, TweenTime);
		
		await ToSignal(tween, Tween.SignalName.Finished);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
