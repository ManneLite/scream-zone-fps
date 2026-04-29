using Godot;
using System;

public partial class EnemySpawner : Node3D
{

	[Export] public PackedScene EnemyType;
	[Export] public float SpawnFrequency = 5.0f;
	[Export] public float Radius = 20.0f;
	[Export] public int SpawnCount = 1;

	public SinType Sin;

	public override void _Ready()
	{
		Timer spawn_timer = GetNode<Timer>("Timer");
		spawn_timer.WaitTime = SpawnFrequency;
		spawn_timer.Timeout += OnTimerTimeout;
		spawn_timer.Start();
	}

	public void OnTimerTimeout()
	{
		// TODO(manne): And is in radius/same chunk maybe?
		Vector3 SpawnPos = GlobalPosition;
		for(int i = 0; i < SpawnCount; ++i)
		{
			if(EnemyType.Instantiate() is EnemyBody3D enemy)
			{
				SpawnPos.X += (float)GD.RandRange(-Radius, Radius);
				SpawnPos.Z += (float)GD.RandRange(-Radius, Radius);
				SpawnPos.Y = 2 + GlobalNoise.Instance.GetYAtPosV3(SpawnPos);

				GetTree().CurrentScene.AddChild(enemy);
				enemy.GlobalPosition = SpawnPos;
			}
		}
	}



}
