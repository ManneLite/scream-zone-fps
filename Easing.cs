using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Easing : Node
{
	public static Easing Instance;

	private readonly Dictionary<GodotObject, Tween> activeTweens = new();

	public override void _Ready()
	{
		Instance = this;
	}

	public async Task AsyncTween(
		GodotObject objectToTween,
		string property,
		Variant goal,
		float tweenTime,
		Tween.TransitionType transition,
		Tween.EaseType easeType,
		bool looped = false)
	{
		if (objectToTween == null)
			return;

		StopTween(objectToTween);

		var tween = CreateTween();
		activeTweens[objectToTween] = tween;

		tween.TweenProperty(objectToTween, property, goal, tweenTime)
			.SetTrans(transition)
			.SetEase(easeType);

		if (looped)
		{
			tween.SetLoops();
			return;
		}

		await ToSignal(tween, Tween.SignalName.Finished);

		if (activeTweens.TryGetValue(objectToTween, out var stored) && stored == tween)
		{
			activeTweens.Remove(objectToTween);
		}
	}

	public struct TweenStep
	{
		public GodotObject ObjectToTween;
		public string Property;
		public Variant Goal;
		public float TweenTime;
		public Tween.TransitionType Transition;
		public Tween.EaseType EaseType;
	}

	public async Task AsyncTweenSequence(List<TweenStep> steps, bool looped = false)
	{
		if (steps == null || steps.Count == 0)
			return;

		do
		{
			foreach (var step in steps)
			{
				if (step.ObjectToTween == null)
					continue;

				StopTween(step.ObjectToTween);

				var tween = CreateTween();
				activeTweens[step.ObjectToTween] = tween;

				tween.TweenProperty(step.ObjectToTween, step.Property, step.Goal, step.TweenTime)
					.SetTrans(step.Transition)
					.SetEase(step.EaseType);

				await ToSignal(tween, Tween.SignalName.Finished);

				if (activeTweens.TryGetValue(step.ObjectToTween, out var stored) && stored == tween)
				{
					activeTweens.Remove(step.ObjectToTween);
				}
			}

		} while (looped);
	}

	public void StopTween(GodotObject obj)
	{
		if (obj == null)
			return;

		if (activeTweens.TryGetValue(obj, out var tween))
		{
			tween.Kill();
			activeTweens.Remove(obj);
		}
	}

	public void StopAllTweens()
	{
		foreach (var tween in activeTweens.Values)
		{
			tween.Kill();
		}

		activeTweens.Clear();
	}
}
