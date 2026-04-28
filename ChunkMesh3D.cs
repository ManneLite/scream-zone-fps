using Godot;
using System;
using System.Threading.Tasks;

public partial class ChunkMesh3D : MeshInstance3D
{	
    public SinType Sin;
	public int Size;
	public Vector2I Pos;

	public override void _Ready()
	{
		Generate();
	}

	private void Generate()
	{
		int vertex_count = (Size + 1) * (Size + 1);
		int indices_count = Size * Size * 6;

		Vector2 offset = Size * (Pos - new Vector2(0.5f, 0.5f));

		Vector3[] vertices = new Vector3[vertex_count];
		Vector2[] uvs = new Vector2[vertex_count];
		int[] indices = new int[indices_count];
		for (int z = 0; z <= Size; z++)
		{
			for (int x = 0; x <= Size; x++)
			{
				Vector2 pos_with_offset = offset + new Vector2I(x,z);
				float y = GlobalNoise.Instance.GetYAtPosV2(pos_with_offset);

				int i = x + z * (Size + 1);

				vertices[i] = new Vector3(pos_with_offset.X, y, pos_with_offset.Y);
				uvs[i] = new Vector2((float)x / (Size - 1), (float)z / (Size - 1));
			}
		}

		int index = 0;

		for (int z = 0; z < Size; z++)
		{
			for (int x = 0; x < Size; x++)
			{
				int i = x + z * (Size+1);

				int a = i;
				int b = i + 1;
				int c = i + Size + 1;
				int d = i + Size + 2;

				indices[index++] = b;
				indices[index++] = c;
				indices[index++] = a;

				indices[index++] = b;
				indices[index++] = d;
				indices[index++] = c;
			}
		}

		var arrays = new Godot.Collections.Array();
		arrays.Resize((int)Mesh.ArrayType.Max);

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
		if (Mesh == null)
			return;

		var body = new StaticBody3D();
		body.SetCollisionLayerValue(4, true);

		var collider = new CollisionShape3D();
		collider.Shape = Mesh.CreateTrimeshShape();

		body.AddChild(collider);

		AddChild(body);
	}
}
