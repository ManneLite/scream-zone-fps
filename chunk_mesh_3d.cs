using Godot;
using System;

public partial class chunk_mesh_3d : MeshInstance3D
{
	[Export] public int Width = 256;
	[Export] public int Height = 256;
	[Export] public float HeightMultiplier = 10f;
	[Export] public float CellSize = 1f;

	[Export] public float NoiseFrequency = 0.01f;
	[Export] public int NoiseOctaves = 4;
	[Export] public float NoiseGain = 0.5f;
	[Export] public float NoiseLacunarity = 2.0f;

	[Export] public bool GenerateOnReady = true;

	public override void _Ready()
	{
		if (GenerateOnReady)
			Generate();
	}

	public void Generate()
	{
		var noise = new FastNoiseLite();
		noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
		noise.Frequency = NoiseFrequency;
		noise.FractalOctaves = NoiseOctaves;
		noise.FractalGain = NoiseGain;
		noise.FractalLacunarity = NoiseLacunarity;

		var arrays = new Godot.Collections.Array();
		arrays.Resize((int)Mesh.ArrayType.Max);

		Vector3[] vertices = new Vector3[Width * Height];
		Vector2[] uvs = new Vector2[Width * Height];
		int[] indices = new int[(Width - 1) * (Height - 1) * 6];

		for (int z = 0; z < Height; z++)
		{
			for (int x = 0; x < Width; x++)
			{
				float n = noise.GetNoise2D(x, z);
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

/*

using Godot;
using Godot.Collections;

public partial class chunk_mesh_3d : MeshInstance3D
{
	Array surface_array = [];
	Vector3 vertices = [216];
	Vector3 normals = [216];
	Color colors = [216];

	Array<Vector3> cube_vertices = [
		new(-0.5f, -0.5f, 0.5f),
		new(0.5f, -0.5f, 0.5f),
		new(0.5f, -0.5f, -0.5f),
		new(-0.5f, -0.5f, -0.5f),
		new(-0.5f, 0.5f, 0.5f),
		new(0.5f, 0.5f, 0.5f),
		new(0.5f, 0.5f, -0.5f),
		new(-0.5f, 0.5f, -0.5f),
	];

	enum Face
	{
		Bottom, Front, Right, Top, Left, Back
	}

	Dictionary<Face, Array> face_indices = new Dictionary<Face, Array>{
		{Face.Front, new Array{new Array{0, 4, 5}, new Array{0, 5, 1}}},
		{Face.Back, new Array{new Array{2, 7, 3}, new Array{2, 6, 7}}},
		{Face.Left, new Array{new Array{3, 7, 4}, new Array{3, 4, 0}}},
		{Face.Right, new Array{new Array{1, 5, 6}, new Array{1, 6, 2}}},
		{Face.Bottom, new Array{new Array{0, 1, 2}, new Array{0, 2, 3}}},
		{Face.Top, new Array{new Array{4, 7, 6}, new Array{4, 6, 5}}},
	};

	Dictionary<Face, Vector3> face_normals = new Dictionary<Face, Vector3>{
		{Face.Front, new Vector3(0, 0, 1)},
		{Face.Back, new Vector3(0, 0, -1)},
		{Face.Left, new Vector3(-1, 0, 0)},
		{Face.Right, new Vector3(1, 0, 0)},
		{Face.Bottom, new Vector3(0, -1, 0)},
		{Face.Top, new Vector3(0, 1, 0)},
	};

	Dictionary<Face, Color> face_colors = new Dictionary<Face, Color>{
		{Face.Front, Colors.Orange},
		{Face.Back, Colors.Purple},
		{Face.Left, Colors.Blue},
		{Face.Right, Colors.Yellow},
		{Face.Bottom, Colors.Red},
		{Face.Top, Colors.Green},
	};

	public override void _Ready()
	{
		surface_array.Resize((int)Mesh.ArrayType.Max);
		mesh_generate();
	}

	void face_add(Face face, Vector3 pos)
	{
		var indices = face_indices[face];
		for(int indices_index = 0; indices_index < 2; ++indices_index)
		{
			var triangle = indices[indices_index];
			for(int index_index = 0; index_index < 3; ++index_index)
			{
				var index = triangle[index_index];
				vertices[] = (cube_vertices[index] + pos);
				normals[] = (face_normals[face]);
				colors[] = (face_colors[face]);
			}
		}
	}

	void mesh_generate()
	{
		// 6 * 6 * 2 * 3
		face_add(Face.Front, Vector3.Zero);
		face_add(Face.Back, Vector3.Zero);
		face_add(Face.Left, Vector3.Zero);
		face_add(Face.Right, Vector3.Zero);
		face_add(Face.Top, Vector3.Zero);
		face_add(Face.Bottom, Vector3.Zero);
		mesh_commit();
	}

	void mesh_commit()
	{
		surface_array[(int)Mesh.ArrayType.Vertex] = vertices;
		surface_array[(int)Mesh.ArrayType.Normal] = normals;
		surface_array[(int)Mesh.ArrayType.Color] = colors;
		if(this.Mesh is ArrayMesh array_mesh)
		{
			array_mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surface_array);
		}
	}
}
*/
