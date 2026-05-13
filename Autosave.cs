using Godot;

public partial class Autosave : Node
{
    [Export]
    public bool Enabled { get; set; } = true;

    [Export]
    public int IntervalMs { get; set; } = 2 * 60 * 1000;

    private Timer? timer;

    public override void _Ready()
    {
        if (!Enabled)
            return;

        timer = new Timer { OneShot = false };
        AddChild(timer);
        timer.Timeout += OnTimeout;
        timer.Start(IntervalMs / 1000.0);
    }

    private async void OnTimeout()
    {
        await GameStore.SaveGameAsync();
    }
}
