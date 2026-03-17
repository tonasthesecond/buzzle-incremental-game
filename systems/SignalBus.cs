using Godot;

public partial class SignalBus : Node
{
    public static SignalBus Instance { get; private set; } = null!;

    [Signal]
    public delegate void ResourceSelectedEventHandler(Resource resource);

    [Signal]
    public delegate void ResourceUnselectedEventHandler();

    public override void _Ready()
    {
        Instance = this;
    }
}
