using Godot;

[GlobalClass]
public partial class PlacementSystem : GameSystem
{
    [Signal]
    public delegate void ModeChangedEventHandler(Mode mode);

    private PackedScene? selectedScene;
    private BaseGridObject? highlighted;
    private const float HighlightAlpha = 0.7f;

    public enum Mode
    {
        None,
        Tile,
        Object,
        Bee,
        RemoveTile,
        RemoveObject,
    }

    private Mode curMode = Mode.None;
    public Mode CurMode
    {
        get => curMode;
        set
        {
            ClearHighlight();
            curMode = value;
            EmitSignal(SignalName.ModeChanged, (int)value);
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
            else if (instance is Bee)
                CurMode = Mode.Bee;
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

    public override void _Process(double delta)
    {
        var grid = Services.Get<Grid>();
        var tilemap = Services.Get<Tilemap>();
        var cell = tilemap.LocalToMap(tilemap.GetLocalMousePosition());

        switch (CurMode)
        {
            case Mode.Bee:
                SetHighlight(
                    grid.GetClosestObjectOfType<HiveGridObject>(grid.GridToWorld(cell)),
                    "highlight_target"
                );
                break;

            case Mode.RemoveObject:
            case Mode.RemoveTile:
                SetHighlight(grid.GetObjectAt(cell), "highlight_delete");
                break;

            default:
                ClearHighlight();
                break;
        }
    }

    private void SetHighlight(BaseGridObject? next, string colorKey)
    {
        if (highlighted != null && highlighted != next)
        {
            if (IsInstanceValid(highlighted))
                highlighted.Modulate = Colors.White;
            highlighted = null;
        }
        highlighted = next;
        if (highlighted != null && IsInstanceValid(highlighted))
        {
            var c = new Color(GameStore.Colors[colorKey]) { A = HighlightAlpha };
            highlighted.Modulate = c;
        }
    }

    private void ClearHighlight()
    {
        if (highlighted != null && IsInstanceValid(highlighted))
            highlighted.Modulate = Colors.White;
        highlighted = null;
    }

    /// Handle tile placement on click.
    public override void _UnhandledInput(InputEvent e)
    {
        if (CurMode == Mode.None)
            return;
        if (e is not InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true })
            return;

        Grid grid = Services.Get<Grid>()!;
        Tilemap tilemap = Services.Get<Tilemap>()!;
        Vector2I pos = tilemap.LocalToMap(tilemap.GetLocalMousePosition());
        FailMessage? fail = null;

        switch (CurMode)
        {
            case Mode.Tile:
                if (selectedScene != null)
                    grid.PlaceTile(selectedScene, pos, out fail);
                break;

            case Mode.Object:
                if (selectedScene != null)
                    grid.PlaceObject(selectedScene, pos, out fail);
                break;

            case Mode.Bee:
                if (highlighted is HiveGridObject hive)
                    Services.Get<BeeSystem>().SpawnBee(hive);
                break;

            case Mode.RemoveTile:
                grid.RemoveTile(pos, out fail);
                break;

            case Mode.RemoveObject:
                grid.RemoveObject(pos, out fail);
                break;
        }

        if (fail != null)
        {
            GD.Print($"[PlacementSystem] {fail.Log}");
            Services.Get<ErrorLabel>().ShowError(fail);
        }
    }
}
