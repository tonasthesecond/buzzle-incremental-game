using Godot;

public partial class SignalBus : Node
{
    public static SignalBus Instance { get; private set; } = null!;

    [Signal]
    public delegate void GameLoadedEventHandler();

    [Signal]
    public delegate void ResourceSelectedEventHandler(Resource resource);

    [Signal]
    public delegate void SelectedResourceSelectedEventHandler(SelectedResource selected);

    [Signal]
    public delegate void ResourceUnselectedEventHandler();

    [Signal]
    public delegate void HoveredEventHandler(Node target);

    [Signal]
    public delegate void UnhoveredEventHandler();

    [Signal]
    public delegate void TilePlacedEventHandler(BaseTile tile);

    [Signal]
    public delegate void TileRemovedEventHandler(BaseTile tile);

    [Signal]
    public delegate void GridObjectPlacedEventHandler(BaseGridObject obj);

    [Signal]
    public delegate void GridObjectRemovedEventHandler(BaseGridObject obj);

    public override void _Ready()
    {
        Instance = this;
    }
}
