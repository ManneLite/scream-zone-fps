using Godot;
using System;

public partial class DroopyButton : Godot.Button
{
    private AudioStreamPlayer sound;
    
    public override void _Ready()
    {
        sound = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
    }
    
    public override void _Pressed()
    {
        sound.Play();
    }
}
