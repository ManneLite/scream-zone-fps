using Godot;

public partial class ThroneRing : MeshInstance3D
{
	public Vector3 RotationSpeed = new Vector3(0, 0.1f, 0);
    public override void _Process(double delta)
    {
		RotateObjectLocal(Vector3.Right, RotationSpeed.X * (float)delta);
		RotateObjectLocal(Vector3.Up, RotationSpeed.Y * (float)delta);
		RotateObjectLocal(Vector3.Forward, RotationSpeed.Z * (float)delta);
    }
}
