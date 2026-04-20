using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class PlacementSystem : GameSystem
{
    [Signal]
    public delegate void ModeChangedEventHandler(Mode mode);

    [Export]
    public bool FreePlace = false;

    private PackedScene? selectedScene;
    private Type? selectedType;
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
        RemoveBee,
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
    private RemoveBee? removeBeeResource;

    public override void _Ready()
    {
        SignalBus.Instance.ResourceSelected += (Resource resource) =>
        {
            if (resource is RemoveTile)
                CurMode = Mode.RemoveTile;
            else if (resource is RemoveObject)
                CurMode = Mode.RemoveObject;
            else if (resource is RemoveBee rb)
            {
                removeBeeResource = rb;
                CurMode = Mode.RemoveBee;
                return;
            }
            if (resource is not PackedScene scene)
                return;

            Node instance = scene.Instantiate();
            if (instance is BaseTile)
                CurMode = Mode.Tile;
            else if (instance is BaseGridObject)
                CurMode = Mode.Object;
            else if (instance is Bee)
                CurMode = Mode.Bee;
            else
            {
                instance.QueueFree();
                CurMode = Mode.None;
                return;
            }

            selectedType = instance.GetType();
            instance.QueueFree();
            selectedScene = scene;
        };

        SignalBus.Instance.ResourceUnselected += () =>
        {
            selectedScene = null;
            selectedType = null;
            CurMode = Mode.None;
        };
    }

    // --- Cost ---

    /// Deduct cost if affordable. Returns false + fail on rejection.
    private bool TryCharge(Type? t, out FailMessage? fail)
    {
        fail = null;
        if (FreePlace || t == null)
            return true;
        int cost = GameStore.GetPlacementCost(t);
        if (cost == 0)
            return true;
        if (GameStore.Honey < cost)
        {
            fail = new FailMessage(
                $"Need {cost} honey",
                $"Insufficient honey for {t.Name}: need {cost}"
            );
            return false;
        }
        GameStore.Honey -= cost;
        return true;
    }

    // Highlighting
    public override void _Process(double delta)
    {
        var grid = Services.Get<Grid>();
        var tilemap = Services.Get<Tilemap>();
        var cell = tilemap.LocalToMap(tilemap.GetLocalMousePosition());

        switch (CurMode)
        {
            case Mode.Bee:
                SetHighlight(
                    grid.GetClosestObjectOfType<Hive>(grid.GridToWorld(cell)),
                    "highlight_target"
                );
                break;
            case Mode.RemoveObject:
            case Mode.RemoveTile:
                SetHighlight(grid.GetObjectAt(cell), "highlight_delete");
                break;
            case Mode.RemoveBee:
                SetHighlight(
                    grid.GetClosestObjectOfType<Hive>(grid.GridToWorld(cell)),
                    "highlight_delete"
                );
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

    // --- Input ---

    public override void _UnhandledInput(InputEvent e)
    {
        if (CurMode == Mode.None)
            return;
        if (e is not InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true })
            return;

        var grid = Services.Get<Grid>();
        var tilemap = Services.Get<Tilemap>();
        var pos = tilemap.LocalToMap(tilemap.GetLocalMousePosition());
        FailMessage? fail = null;

        switch (CurMode)
        {
            case Mode.Tile:
                if (selectedScene != null && TryCharge(selectedType, out fail))
                    grid.PlaceTile(selectedScene, pos, out fail);
                break;

            case Mode.Object:
                if (selectedScene != null && TryCharge(selectedType, out fail))
                    grid.PlaceObject(selectedScene, pos, out fail);
                break;

            case Mode.Bee:
                if (
                    selectedScene != null
                    && highlighted is Hive hive
                    && TryCharge(selectedType, out fail)
                )
                    Services.Get<BeeSystem>().SpawnBee(selectedScene, hive);
                break;

            case Mode.RemoveTile:
                grid.RemoveTile(pos, out fail);
                break;

            case Mode.RemoveObject:
                grid.RemoveObject(pos, out fail);
                break;

            case Mode.RemoveBee:
                if (highlighted is Hive targetHive && removeBeeResource != null)
                {
                    string beeType = removeBeeResource.BeeTypeName;

                    Type t = beeType switch
                    {
                        "" => typeof(BaseBee),
                        "Queen" => typeof(QueenBee),
                        "Rocket" => typeof(RocketBee),
                        "Jetpack" => typeof(RocketBee),
                        "Fat" => typeof(FatBee),
                        _ => throw new ArgumentException($"Unknown bee type: {beeType}"),
                    };

                    if (!Services.Get<BeeSystem>().RemoveBee(t, targetHive))
                    {
                        if (beeType == "Rocket")
                            beeType = "Jetpack";
                        fail = new FailMessage(
                            $"No {beeType} bee to remove.",
                            $"No {beeType} bee found at hive!"
                        );
                    }
                }
                break;
        }

        if (fail != null)
        {
            GD.Print($"[PlacementSystem] {fail.Log}");
            Services.Get<ErrorLabel>().ShowError(fail);
        }
    }
}
