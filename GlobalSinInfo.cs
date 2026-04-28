using Godot;
using System;
using System.Collections.Generic;

public partial class GlobalSinInfo : Node
{
    public static GlobalSinInfo Instance;

	[Export] public Godot.Collections.Dictionary<SinType, StandardMaterial3D> SinToShader = new();
	public Dictionary<Vector2, SinType> ChunkPosToSin = new();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Instance = this;

		ChunkPosToSin[new Vector2(0,0)] = SinType.Divine;
		ChunkPosToSin[new Vector2(1,0)] = SinType.Empty;
		ChunkPosToSin[new Vector2(-1,0)] = SinType.Lust;
		ChunkPosToSin[new Vector2(0,1)] = SinType.Envy;
		ChunkPosToSin[new Vector2(0,-1)] = SinType.Gluttony;
		ChunkPosToSin[new Vector2(1,1)] = SinType.Wrath;
		ChunkPosToSin[new Vector2(-1,-1)] = SinType.Sloth;
		ChunkPosToSin[new Vector2(-1,1)] = SinType.Pride;
		ChunkPosToSin[new Vector2(1,-1)] = SinType.Greed;
	}

    public SinType GetSinByChunkPos(Vector2 pos)
    {
        return ChunkPosToSin[pos];
    }

    public StandardMaterial3D GetShaderBySin(SinType sin)
    {
        return SinToShader[sin];
    }

    public StandardMaterial3D GetShaderByChunkPos(Vector2 pos)
    {
        return GetShaderBySin(GetSinByChunkPos(pos));
    }

}
