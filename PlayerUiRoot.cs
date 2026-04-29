using Godot;
using System;

public partial class PlayerUiRoot : Control
{
    Control EscapeMenu;

	public override void _Ready()
	{
        EscapeMenu = GetNode<Control>("MenuEscape");
	}

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_pause"))
        {
            if (!GetTree().IsPaused())
            {
                GetTree().Paused = true;
                EscapeMenu.Show();
		        Input.MouseMode = Godot.Input.MouseModeEnum.Visible;
                GetViewport().SetInputAsHandled();
            }
        }
    }
}
