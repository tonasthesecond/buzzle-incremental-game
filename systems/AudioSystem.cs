using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class AudioSystem : GameSystem
{
    [Export]
    public int Volume { get; set; } = 10;

    public AudioStreamPlayer AudioPlayer { get; set; }

    private AudioStreamWav bgTrack = GD.Load<AudioStreamWav>("res://assets/audio/bg.wav");

    private readonly Dictionary<string, AudioStream> sounds = new Dictionary<string, AudioStream>()
    {
        { "start", GD.Load<AudioStream>("uid://6tp7kbfu0gms") },
        { "ding", GD.Load<AudioStream>("uid://b0u7vsk62q327") },
        { "click", GD.Load<AudioStream>("uid://doatihjqmox7n") },
        { "break", GD.Load<AudioStream>("uid://celhhaufgchlu") },
        { "place", GD.Load<AudioStream>("uid://cwpp0crry1bct") },
        { "place_big", GD.Load<AudioStream>("uid://dya7rogmog5ux") },
        { "error", GD.Load<AudioStream>("uid://cegrrmtluuygg") },
        { "upgrade", GD.Load<AudioStream>("uid://da0w7vskrcfh6") },
    };

    public override void _Ready()
    {
        AudioPlayer = new AudioStreamPlayer();
        AddChild(AudioPlayer);
        AudioPlayer.Stream = bgTrack;
        AudioPlayer.VolumeDb = StepToDb(Volume);
        AudioPlayer.Play();
    }

    public float StepToDb(int step, int max = 10, float minDb = -80f)
    {
        if (step <= 0)
            return -80f;
        return Mathf.Lerp(minDb, 0f, step / (float)max);
    }

    public void PlaySound(string name, float volume = 1f)
    {
        if (!sounds.TryGetValue(name, out var stream))
            return;

        var player = new AudioStreamPlayer();
        AddChild(player);
        player.Stream = stream;
        player.VolumeDb = Mathf.LinearToDb(volume);
        player.Finished += player.QueueFree;
        player.Play();
    }
}
