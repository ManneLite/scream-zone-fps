using Godot;

public partial class MenuMain : Control
{
    [Export] public PackedScene GameLevel;

    Control CreditsScreen;
    Control MenuButtons;
    Control Spooky_JPG;
    Control normal_menu;
    AudioStreamPlayer sound;
    bool been_jumpscared = false;

    override public void _Ready()
    {
        normal_menu = GetNode<Control>("Book");
        Spooky_JPG = GetNode<Control>("Spooky_JPG");
        CreditsScreen = GetNode<Control>("Book/CreditsScreen");
        MenuButtons = GetNode<Control>("Book/MenuButtons");
        sound = GetNode<AudioStreamPlayer>("Spooky_JPG/AudioStreamPlayer");
        sound.Finished += () => {Spooky_JPG.Hide(); normal_menu.Show();};

        Spooky_JPG.Hide();
        CreditsScreen.Hide();
        MenuButtons.Show();


    }

    public void OnTitlePressed()
    {
        if(!been_jumpscared)
        {
            sound.Play();
            normal_menu.Hide();
            Spooky_JPG.Show();
        }

        been_jumpscared = true;
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
