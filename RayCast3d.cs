using Godot;
using System;

public partial class RayCast3d : RayCast3D
{
	
	private Vector2 OldLocal;
	
	[Signal] public delegate void PlayerChangedChunkEventHandler(Vector2 Local);
	
	public override void _Ready()
	{
		Enabled = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsColliding())
			return;

		var collider = GetCollider() as Node;
		if (collider == null)
			return;

		var parent = collider.GetParent();

		if (parent is chunk_mesh_3d chunk)
		{
			Vector2 a = new(chunk.LocalPosX, chunk.LocalPosZ);
			
			if (OldLocal != a)
			{
				GD.Print("Updating chunks!");
				EmitSignal(SignalName.PlayerChangedChunk, a);
				OldLocal = a;
			}
		}
	}
}
