using Godot;
using System;

public partial class SaveFile : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Game's opening!");
		ConfigFile Config = new();
		Config.Load("user://save.cfg");
		
		Godot.Variant a = Config.GetValue("player", "randomNumber");
		GD.Print(a);
	}
	
	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
		{
			GD.Print("Game's closing!");
			RandomNumberGenerator rng = new();
			int a = (int)rng.Randi();
		
			GD.Print(a);
		
			ConfigFile Config = new();
		
			Config.SetValue("player", "randomNumber", a);
		
			Config.Save("user://save.cfg");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
