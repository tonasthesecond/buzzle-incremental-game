using Godot;

public partial class HoverAreaComponent : Control
{
    [Export]
    public float HoverDelay { get; set; } = 0f;

    private float elapsed = 0f;
    private bool hovering = false;
    private bool fired = false;

    public override void _Ready()
    {
        MouseEntered += () =>
        {
            hovering = true;
            fired = false;
        };
        MouseExited += () =>
        {
            hovering = false;
            elapsed = 0f;
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.Unhovered);
        };
    }

    public override void _Process(double delta)
    {
        if (!hovering || fired)
            return;
        elapsed += (float)delta;
        if (elapsed >= HoverDelay)
        {
            fired = true;
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.Hovered, Owner);
        }
    }

    public void Setup(Node owner, float hoverDelay)
    {
        Owner = owner;
        HoverDelay = hoverDelay;
    }
}
