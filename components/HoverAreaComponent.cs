using System.Linq;
using Godot;

public partial class HoverAreaComponent : Control
{
    public override void _Ready()
    {
        MouseEntered += () => SignalBus.Instance.EmitSignal(SignalBus.SignalName.Hovered, Owner);
        MouseExited += () => SignalBus.Instance.EmitSignal(SignalBus.SignalName.Unhovered);
    }
}
