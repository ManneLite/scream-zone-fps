using Godot;
using System;

public partial class ObjectSpawner : Node
{	
	public Vector3 GlobalPosition;
	private ChunkMesh3D Chunk;
	private PlaceableObjects Objects;
	
	public override void _Ready()
	{
		Chunk = (ChunkMesh3D)GetParent();
		Objects = GetTree().CurrentScene.GetNode<PlaceableObjects>("PlaceableObjects");
		SpawnObjects(30);
	}
	
	public void SpawnObjects(int amount) {
		for (int i = 0; i <= amount; i++)
		{
			Vector3 Position = RandomPosition(Chunk.Size);
			PlacePackedSceneObject(Position, Objects.FreakySkull);
		}
	}
	
	private void PlacePackedSceneObject(Vector3 Position, PackedScene PackedSceneObject)
	{
		StaticBody3D InstantiatedObject = (StaticBody3D)PackedSceneObject.Instantiate();
		Position += (Vector3)new(0.0f,InstantiatedObject.GetNode<MeshInstance3D>("MeshInstance3D").Mesh.GetAabb().Size.Y * 0.1f,0.0f);
		InstantiatedObject.Position = Position;
		InstantiatedObject.Rotation = (Vector3)new(0,(float)GD.RandRange(-90, 90),0);
		Chunk.AddChild(InstantiatedObject);
	}
	
	private Vector3 RandomPosition(int size)
	{
		Vector3 Position = GlobalPosition;
        float bordered_half_size = (size/2) - 2;
		Position.X += (float)GD.RandRange(-bordered_half_size, bordered_half_size);
		Position.Z += (float)GD.RandRange(-bordered_half_size, bordered_half_size);
		Position.Y = GlobalNoise.Instance.GetYAtPosV3(Position);
		
		return Position;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
