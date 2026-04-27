using Godot;
using System.Collections.Generic;

public partial class AudioSystem3D : Node3D
{
    public static AudioSystem3D Instance {get; private set;}

    [Export] public int PoolSize = 8;
    [Export] public string Bus = "Master";

    public record PlayInfo(AudioStreamMP3 stream, Vector3 pos, float offset);

    public Stack<AudioStreamPlayer3D> available_3d;
    public Queue<PlayInfo> queue;

    override public void _Ready()
    {
        Instance = this;
        available_3d = new(PoolSize);
        queue = new();
        for(int index = 0; index < PoolSize; ++index)
        {
            AudioStreamPlayer3D player = new();
            AddChild(player);
            available_3d.Push(player);
            GD.Print(available_3d.Count);
            player.Finished += () => OnStreamFinished(player);
            player.Bus = Bus;
        }
    }

    void OnStreamFinished(AudioStreamPlayer3D player)
    {
        available_3d.Push(player);
    }

    public void PlayAt(AudioStreamMP3 stream, Vector3 pos, float offset = -1.0f)
    {
        queue.Enqueue(new(stream, pos, offset));
    }

    override public void _Process(double delta)
    {
        if(queue.Count != 0 && available_3d.Count != 0)
        {
            AudioStreamPlayer3D player = available_3d.Pop();
            PlayInfo info = queue.Dequeue();
            player.GlobalPosition = info.pos;
            player.Stream = info.stream;
            player.Play(info.offset);
            GD.Print("Playing audio at: " + info.pos);
        }
    }
}
