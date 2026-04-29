using Godot;
using System;
using System.Collections.Generic;

public partial class BiomeLabel : Label
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var Player = (Player3D)GetTree().GetFirstNodeInGroup("Player");
		Player.PlayerChangedChunks += UpdateLabel;
	}
	
	public void UpdateLabel(string text)
	{
		Text = text;
		var sequence = new List<Easing.TweenStep>
			{
				new Easing.TweenStep
				{
					ObjectToTween = this,
					Property = "modulate",
					Goal = new Color(1.0f, 1.0f, 1.0f, 1.0f),
					TweenTime = 0.5f,
					Transition = Tween.TransitionType.Sine,
					EaseType = Tween.EaseType.InOut
				},
				new Easing.TweenStep
				{
					ObjectToTween = this,
					Property = "modulate",
					Goal = new Color(1f, 1f, 1f, 0f),
					TweenTime = 0.5f,
					Transition = Tween.TransitionType.Sine,
					EaseType = Tween.EaseType.InOut
				}
			};

		Easing.Instance.AsyncTweenSequence(sequence, false);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
