using Godot;

public partial class MenuMain : Control
{
    [Export] public PackedScene GameLevel;

    public void OnPlayButtonPressed()
    {
        GetTree().ChangeSceneToPacked(GameLevel);
    }
}
