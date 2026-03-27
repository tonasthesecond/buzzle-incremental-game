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

    private Mode _mode = Mode.None;
    private Mode mode
    {
        get => _mode;
        set
        {
            _mode = value;
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
                mode = Mode.RemoveTile;
                return;
            }
            if (resource.ResourceName == "RemoveObject")
            {
                mode = Mode.RemoveObject;
                return;
            }
            if (resource is not PackedScene scene)
                return;

            var instance = scene.Instantiate();
            if (instance is BaseTile)
                mode = Mode.Tile;
            else if (instance is BaseGridObject)
                mode = Mode.Object;
            else
            {
                mode = Mode.None;
                return;
            }

            selectedScene = scene;
        };

        SignalBus.Instance.ResourceUnselected += () =>
        {
            selectedScene = null;
            mode = Mode.None;
        };
    }

    /// Handle tile placement on click.
    public override void _UnhandledInput(InputEvent e)
    {
        if (mode == Mode.None)
            return;
        if (e is not InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true })
            return;

        // operate based on mode
        var grid = Services.Get<Grid>();
        var tilemap = Services.Get<Tilemap>();
        var pos = tilemap.LocalToMap(tilemap.GetLocalMousePosition());
        switch (mode)
        {
            case Mode.Tile:
                if (selectedScene == null)
                    return;
                if (!grid.PlaceTile(selectedScene, pos, out FailMessage? tileFail))
                    Services.Get<ErrorLabel>().ShowError(tileFail);
                if (tileFail != null)
                    GD.Print($"[PlacementSystem] {tileFail.Log}");
                break;

            case Mode.Object:
                if (selectedScene == null)
                    return;
                if (!grid.PlaceObject(selectedScene, pos, out FailMessage? objFail))
                    Services.Get<ErrorLabel>().ShowError(objFail);
                if (objFail != null)
                    GD.Print($"[PlacementSystem] {objFail.Log}");
                break;

            case Mode.RemoveTile:
                if (!grid.RemoveTile(pos, out FailMessage? removeTileFail))
                    Services.Get<ErrorLabel>().ShowError(removeTileFail);
                if (removeTileFail != null)
                    GD.Print($"[PlacementSystem] {removeTileFail.Log}");
                break;

            case Mode.RemoveObject:
                if (!grid.RemoveObject(pos, out FailMessage? removeObjFail))
                    Services.Get<ErrorLabel>().ShowError(removeObjFail);
                if (removeObjFail != null)
                    GD.Print($"[PlacementSystem] {removeObjFail.Log}");
                break;
        }
    }
}
