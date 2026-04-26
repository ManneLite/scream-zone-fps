using Godot;
using System;
using System.Threading.Tasks;

public partial class chunk_mesh_3d : MeshInstance3D
{
	public int Size;
    public Vector2I Pos;
	public float Height;

	public FastNoiseLite Noise;

	public override void _Ready()
	{
		Generate();
	}

    // .-.-0-.-.
    //
    // 0-.-.-.
    // 0-.-.-.-.
    //
    // .---.---.---.---.
    // |
    // .
    // |
    // .
    // |
    // .
    // |
    // .

    // 2 x 2
    // 1 x 1 * 6
    // 3 x 3
    // 2 x 2 * 6


	private void Generate()
	{
        // offset is now actually where our center should be
        Vector2 offset = Pos * (Size);
		Vector3[] vertices = new Vector3[(Size + 1) * (Size + 1)];
		Vector2[] uvs = new Vector2[(Size + 1) * (Size + 1)];
		int[] indices = new int[Size * Size * 6];
		for (int z = 0; z <= Size; z++)
		{
			for (int x = 0; x <= Size; x++)
			{
                Vector2 pos_with_offset = offset + new Vector2I(x,z);
				float n = Noise.GetNoise2Dv(pos_with_offset);
				n = (n + 1f) * 0.5f;
				float y = n * Height;

				int i = x + z * (Size + 1);

				vertices[i] = new Vector3(pos_with_offset.X - Size/2, y, pos_with_offset.Y - Size/2);
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
