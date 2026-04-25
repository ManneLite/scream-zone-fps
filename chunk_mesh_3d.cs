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

	[Export] public PackedScene Enemy_Default;
    [Export] public CharacterBody3D Player;
    public FastNoiseLite Noise;

	public override void _Ready()
	{
		Noise = new FastNoiseLite();
		Noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
		Noise.Frequency = NoiseFrequency;
		Noise.FractalOctaves = NoiseOctaves;
		Noise.FractalGain = NoiseGain;
		Noise.FractalLacunarity = NoiseLacunarity;
		if (GenerateOnReady)
			Generate();
	}

	public void Generate()
	{

		var arrays = new Godot.Collections.Array();
		arrays.Resize((int)Mesh.ArrayType.Max);

		Vector3[] vertices = new Vector3[Width * Height];
		Vector2[] uvs = new Vector2[Width * Height];
		int[] indices = new int[(Width - 1) * (Height - 1) * 6];

		for (int z = 0; z < Height; z++)
		{
			for (int x = 0; x < Width; x++)
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

    public void _on_spawn_enemy_timer()
    {
        if(Enemy_Default.Instantiate() is enemy_body_3d enemy)
        {
            float enemy_pos_x = (float)GD.RandRange(10.0f, Width-10.0f);
            float enemy_pos_z = (float)GD.RandRange(10.0f, Height-10.0f);
			float n = Noise.GetNoise2D(enemy_pos_x, enemy_pos_z);
			n = (n + 1f) * 0.5f;
			float enemy_pos_y = (n * HeightMultiplier) + 2;

            enemy.Target = Player;
            GetTree().CurrentScene.AddChild(enemy);
            enemy.GlobalPosition = new Vector3(enemy_pos_x, enemy_pos_y, enemy_pos_z);
            GD.Print("Enemy spawned at: " + enemy.GlobalPosition);
        }
    }
}
