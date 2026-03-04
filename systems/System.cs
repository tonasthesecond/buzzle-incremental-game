using Godot;

/// <summary>
/// Base class for all systems.
/// </summary>
public abstract partial class GameSystem : Node
{
    [Export]
    public bool enabled = true;

    public override void _Ready()
    {
        if (!enabled)
            QueueFree();
    }
}
