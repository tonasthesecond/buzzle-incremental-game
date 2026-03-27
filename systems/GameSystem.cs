using Godot;

/// Base class for all systems.
public abstract partial class GameSystem : Node
{
    // allow the system to be disabled
    [Export]
    public bool enabled = true;

    public override void _EnterTree()
    {
        if (enabled)
            Services.Register(this);
    }
}
