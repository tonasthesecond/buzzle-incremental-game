using System;
using Godot;

[GlobalClass]
public partial class AudioSystem : GameSystem
{
    [Export]
    public int Volume { get; set; } = 10;

    public AudioStreamPlayer AudioPlayer { get; set; }

    private AudioStreamWav bgTrack = GD.Load<AudioStreamWav>("res://assets/audio/bg.wav");

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
        AudioPlayer.Stream = GD.Load<AudioStream>("res://assets/audio/" + name + ".wav");
    }
}
