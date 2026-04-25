using Godot;
using System;

public partial class chunk_mesh_3d : MeshInstance3D
{
	public int Width;
	public int Height;
	public float HeightMultiplier;
	public float CellSize;
	
	public int OffsetX;
	public int OffsetY;
	
	public FastNoiseLite Noise;
	
	public override void _Ready()
	{
		Generate();
	}

	public void Generate()
	{

		var arrays = new Godot.Collections.Array();
		arrays.Resize((int)Mesh.ArrayType.Max);

		Vector3[] vertices = new Vector3[Width * Height];
		Vector2[] uvs = new Vector2[Width * Height];
		int[] indices = new int[(Width - 1) * (Height - 1) * 6];

		for (int z = OffsetY*Height; z < OffsetY*Height+Height; z++)
		{
			for (int x = OffsetX*Width; x < OffsetX*Width+Width; x++)
			{
				float n = Noise.GetNoise2D(x, z);
				n = (n + 1f) * 0.5f;
				float y = n * HeightMultiplier;

				int i = x + z * Width;

				vertices[i] = new Vector3(x * CellSize, y, z * CellSize);
				uvs[i] = new Vector2((float)x / (Width - 1), (float)z / (Height - 1));
			}
		}

		int index = 0;

		for (int z = 0; z < Height - 1; z++)
		{
			for (int x = 0; x < Width - 1; x++)
			{
				int i = x + z * Width;

				int a = i;
				int b = i + 1;
				int c = i + Width;
				int d = i + Width + 1;

				indices[index++] = b;
				indices[index++] = c;
				indices[index++] = a;

				indices[index++] = b;
				indices[index++] = d;
				indices[index++] = c;
			}
		}

		arrays[(int)Mesh.ArrayType.Vertex] = vertices;
		arrays[(int)Mesh.ArrayType.TexUV] = uvs;
		arrays[(int)Mesh.ArrayType.Index] = indices;

		var mesh = new ArrayMesh();
		mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

		Mesh = mesh;

		GenerateCollision();
	}

	private void GenerateCollision()
	{
		if (Mesh == null) {
			GD.Print("No Mesh");
			return;
		}

		var body = new StaticBody3D();
		var collider = new CollisionShape3D();
		body.SetCollisionLayerValue(4, true);

		collider.Shape = Mesh.CreateTrimeshShape();

		body.AddChild(collider);
		AddChild(body);
	}
}
