using Godot;

public partial class MenuMain : Control
{
    [Export] public PackedScene GameLevel;

    Control CreditsScreen;
    Control MenuButtons;

    override public void _Ready()
    {
        CreditsScreen = GetNode<Control>("Book/CreditsScreen");
        MenuButtons = GetNode<Control>("Book/MenuButtons");
    }

    public void OnPlayButtonPressed()
    {
        GetTree().Paused = false;

        GetTree().ChangeSceneToPacked(GameLevel);
    }

    public void OnShowCreditsButtonPressed()
    {
        MenuButtons.Hide();
        CreditsScreen.Show();
    }

    public void OnHideCreditsButtonPressed()
    {
        MenuButtons.Show();
        CreditsScreen.Hide();
    }

    public void OnQuitButtonPressed()
    {
        // hook up saving or something
        GetTree().Quit();
    }
}
