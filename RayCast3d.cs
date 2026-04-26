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
        if(IsColliding())
        {
            if(GetCollider() is Node collider && 
               collider.GetParent() is chunk_mesh_3d chunk)
            {
                Vector2 a = chunk.Pos;
                if(OldLocal != a)
                {
				    GD.Print("Updating chunks!");
				    EmitSignal(SignalName.PlayerChangedChunk, a);
				    OldLocal = a;
                }
            }
        }
	}
}
