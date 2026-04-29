using Godot;

public partial class PlayerHud : Control
{
	[Export] public Texture2D HeartTexture;
	public GridContainer HeartsGrid;
	override public void _Ready()
	{
		HeartsGrid = GetNode<GridContainer>("HeartsGrid");
		Initialize(10);
	}

	public void Initialize(int HP)
	{
		HeartsGrid.Columns = HP;
		for(int i = 0; i < HP; ++i)
		{
			TextureRect heart = new();
			HeartsGrid.AddChild(heart);
			heart.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
			heart.Texture = HeartTexture;
			heart.CustomMinimumSize = new(32,32);
		}
	}

	public void UpdateHPBar(int current_hp)
	{
		var hearts = HeartsGrid.GetChildren();
		for(int i = 0; i < HeartsGrid.Columns; ++i)
		{
			if(hearts[i] is CanvasItem heart)
			{
				heart.Visible = (i < current_hp);
			}
		}
	}
}
