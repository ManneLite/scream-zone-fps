using Godot;
using System;

public partial class ChunkCheckRay3D : RayCast3D
{
	
	private Vector2 OldLocal;
	
	[Signal] public delegate void PlayerChangedChunkEventHandler(Vector2I Local, SinType ChunkSin);
	
	public override void _Ready()
	{
		Enabled = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		if(IsColliding())
		{
			if(GetCollider() is Node collider && 
			   collider.GetParent() is ChunkMesh3D chunk)
			{
				Vector2 a = chunk.Pos;
				if(OldLocal != a)
				{
					EmitSignal(SignalName.PlayerChangedChunk, a, (int)chunk.Sin);
					OldLocal = a;
				}
			}
		}
	}
}
