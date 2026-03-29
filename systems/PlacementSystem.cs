using Godot;

[GlobalClass]
public partial class PlacementSystem : GameSystem
{
    [Signal]
    public delegate void OnModeChangedEventHandler(Mode mode);

    private PackedScene? selectedScene;

    public enum Mode
    {
        None,
        Tile,
        Object,
        RemoveTile,
        RemoveObject,
    }

    private Mode curMode = Mode.None;
    public Mode CurMode
    {
        get => curMode;
        set
        {
            curMode = value;
            EmitSignal(SignalName.OnModeChanged, (int)value);
        }
    }

    public override void _Ready()
    {
        // switch mode on resource selection
        SignalBus.Instance.ResourceSelected += (Resource resource) =>
        {
            if (resource.ResourceName == "RemoveTile")
            {
                CurMode = Mode.RemoveTile;
                return;
            }
            if (resource.ResourceName == "RemoveObject")
            {
                CurMode = Mode.RemoveObject;
                return;
            }
            if (resource is not PackedScene scene)
                return;

            var instance = scene.Instantiate();
            if (instance is BaseTile)
                CurMode = Mode.Tile;
            else if (instance is BaseGridObject)
                CurMode = Mode.Object;
            else
            {
                CurMode = Mode.None;
                return;
            }

            selectedScene = scene;
        };

        SignalBus.Instance.ResourceUnselected += () =>
        {
            selectedScene = null;
            CurMode = Mode.None;
        };
    }

    /// Handle tile placement on click.
    public override void _UnhandledInput(InputEvent @event)
    {
        if (CurMode == Mode.None)
            return;
        if (@event is not InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true })
            return;

        // operate based on mode
        Grid grid = Services.Get<Grid>()!;
        Tilemap tilemap = Services.Get<Tilemap>()!;
        Vector2I pos = tilemap.LocalToMap(tilemap.GetLocalMousePosition());
        FailMessage? failMessage = null;

        switch (CurMode)
        {
            case Mode.Tile:
                if (selectedScene == null)
                    return;
                grid.PlaceTile(selectedScene, pos, out failMessage);
                break;

            case Mode.Object:
                if (selectedScene == null)
                    return;
                grid.PlaceObject(selectedScene, pos, out failMessage);
                break;

            case Mode.RemoveTile:
                grid.RemoveTile(pos, out failMessage);
                break;

            case Mode.RemoveObject:
                grid.RemoveObject(pos, out failMessage);
                break;
        }

        if (failMessage != null)
        {
            Services.Get<ErrorLabel>().ShowError(failMessage);
            GD.Print($"[PlacementSystem] {failMessage.Log}");
        }

        GetViewport().SetInputAsHandled();
    }
}
