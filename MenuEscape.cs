using Godot;

public partial class MenuEscape : Control
{
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_pause"))
        {
            if(GetTree().IsPaused())
            {
                resume();
                GetViewport().SetInputAsHandled();
            }
        }
    }

    public void OnResumeButtonPressed()
    {
        resume();
    }

    public void OnGiveUpButtonPressed()
    {
        GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://MenuMain.tscn");
    }

    private void resume()
    {
        Hide();
		Input.MouseMode = Godot.Input.MouseModeEnum.Captured;
        GetTree().Paused = false;
    }
}
