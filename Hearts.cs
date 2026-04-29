using Godot;
using System;
using System.Collections.Generic;

public partial class Hearts : CanvasLayer
{
	public Godot.Collections.Array<Node> HeartObjects;

	public override void _Ready()
	{
		HeartObjects = GetTree().GetNodesInGroup("Hearts");

		foreach (Node heart in HeartObjects)
		{
			StartGlow(heart);
		}

		var player = (Player3D)GetTree().GetFirstNodeInGroup("Player");
		player.PlayerHPChanged += HealthUpdate;
	}

	private async void StartGlow(Node heart)
	{
		var sequence = new List<Easing.TweenStep>
		{
			new Easing.TweenStep
			{
				ObjectToTween = heart,
				Property = "modulate",
				Goal = new Color(1.0f, 0.745f, 1.0f, 1.0f),
				TweenTime = 0.5f,
				Transition = Tween.TransitionType.Sine,
				EaseType = Tween.EaseType.InOut
			},
			new Easing.TweenStep
			{
				ObjectToTween = heart,
				Property = "modulate",
				Goal = new Color(1f, 1f, 1f, 1f),
				TweenTime = 0.5f,
				Transition = Tween.TransitionType.Sine,
				EaseType = Tween.EaseType.InOut
			}
		};

		await Easing.Instance.AsyncTweenSequence(sequence, true);
	}

	public void HealthUpdate(int health)
	{
		foreach (Node heart in HeartObjects)
		{
			int index = int.Parse(heart.Name.ToString());

			if (index > health)
			{
				Easing.Instance.StopTween(heart);

				Easing.Instance.AsyncTween(
					heart,
					"modulate",
					new Color(0.0f, 0.0f, 0.0f, 0.3f),
					0.5f,
					Tween.TransitionType.Sine,
					Tween.EaseType.InOut
				);
			}
		}
	}
}
