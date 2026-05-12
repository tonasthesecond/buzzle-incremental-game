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

    private PlacementExplosionParticles placementParticles = null!;
    private PlacementExplosionParticles removalParticles = null!;

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

    private static readonly Dictionary<Mode, string> PlaceSounds = new()
    {
        { Mode.Tile, "place_big" },
        { Mode.Object, "place" },
        { Mode.Bee, "ding" },
        { Mode.RemoveTile, "break" },
        { Mode.RemoveObject, "break" },
        { Mode.RemoveBee, "break" },
    };

    public override void _Ready()
    {
        placementParticles = GetParent()
            .GetNode<PlacementExplosionParticles>("%PlacementParticles");
        removalParticles = GetParent().GetNode<PlacementExplosionParticles>("%RemovalParticles");

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
                ClearHoverText();
                CurMode = Mode.None;
                return;
            }

            selectedType = instance.GetType();
            UpdateHoverText(selectedType);
            instance.QueueFree();
            selectedScene = scene;
        };

        SignalBus.Instance.ResourceUnselected += () =>
        {
            selectedScene = null;
            selectedType = null;
            ClearHoverText();
            CurMode = Mode.None;
        };

        SignalBus.Instance.RainbowPlaced += (Rainbow rainbow) =>
        {
            CurMode = Mode.None;
            selectedScene = null;
            selectedType = null;
            ClearHoverText();
        };
    }

    /// Deduct cost if affordable. Returns false + fail on rejection.
    private bool TryCharge(Type? t, out int cost, out FailMessage? fail)
    {
        fail = null;
        if (FreePlace || t == null)
        {
            cost = 0;
            return true;
        }
        cost = GameStore.GetPlacementCost(t);
        GD.Print($"Charging {cost} honey");
        if (cost == 0)
            return true;
        if (GameStore.Honey < cost)
        {
            fail = new FailMessage($"Need {cost} honey", $"Need {cost} honey!");
            return false;
        }
        return true;
    }

    private void ChargeHoney(Type? t, int cost)
    {
        GameStore.Honey -= cost;
        UpdateHoverText(t);
    }

    private GameStore.HoneyChangedEventHandler? honeyChangedHandler;
    private SignalBus.GridObjectPlacedEventHandler? gridObjectPlacedHandler;
    private SignalBus.GridObjectRemovedEventHandler? gridObjectRemovedHandler;

    private void UpdateHoverText(Type? t)
    {
        // clear any existing subscription first
        if (honeyChangedHandler != null)
        {
            GameStore.Instance.HoneyChanged -= honeyChangedHandler;
            honeyChangedHandler = null;
        }
        if (gridObjectPlacedHandler != null)
        {
            SignalBus.Instance.GridObjectPlaced -= gridObjectPlacedHandler;
            gridObjectPlacedHandler = null;
        }
        if (gridObjectRemovedHandler != null)
        {
            SignalBus.Instance.GridObjectRemoved -= gridObjectRemovedHandler;
            gridObjectRemovedHandler = null;
        }

        void show()
        {
            bool enough = (t != null) && (GameStore.Honey >= GameStore.GetPlacementCost(t));
            Services
                .Get<HoverLabel>()
                .ShowMessage(
                    "$" + GameStore.GetPlacementCost(t).ToString(),
                    new Color(GameStore.Colors["price_" + (enough ? "" : "not_") + "enough"])
                );
        }

        honeyChangedHandler = (_) => show();
        gridObjectPlacedHandler = (_) => show();
        gridObjectRemovedHandler = (_) => show();
        GameStore.Instance.HoneyChanged += honeyChangedHandler;
        SignalBus.Instance.GridObjectPlaced += gridObjectPlacedHandler;
        SignalBus.Instance.GridObjectRemoved += gridObjectRemovedHandler;
        show();
    }

    public void ClearHoverText()
    {
        if (honeyChangedHandler != null)
        {
            GameStore.Instance.HoneyChanged -= honeyChangedHandler;
            honeyChangedHandler = null;
        }
        if (gridObjectPlacedHandler != null)
        {
            SignalBus.Instance.GridObjectPlaced -= gridObjectPlacedHandler;
            gridObjectPlacedHandler = null;
        }
        if (gridObjectRemovedHandler != null)
        {
            SignalBus.Instance.GridObjectRemoved -= gridObjectRemovedHandler;
            gridObjectRemovedHandler = null;
        }
        Services.Get<HoverLabel>().HideMessage();
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
        Vector2I pos = tilemap.LocalToMap(tilemap.GetLocalMousePosition());
        FailMessage? fail = null;
        int cost = 0;

        bool success = false;

        switch (CurMode)
        {
            case Mode.Tile:
                if (
                    selectedScene != null
                    && TryCharge(selectedType, out cost, out fail)
                    && grid.PlaceTile(selectedScene, pos, out fail)
                )
                {
                    placementParticles.Emit(grid.GridToWorld(pos));
                    ChargeHoney(selectedType, cost);
                    success = true;
                }
                break;

            case Mode.Object:
                Type? objType = selectedType;
                if (
                    selectedScene != null
                    && TryCharge(objType, out cost, out fail)
                    && grid.PlaceObject(selectedScene, pos, out fail)
                )
                {
                    ChargeHoney(objType, cost);
                    placementParticles.Emit(grid.GridToWorld(pos));
                    success = true;
                }
                break;

            case Mode.Bee:
                if (
                    selectedScene != null
                    && highlighted is Hive hive
                    && TryCharge(selectedType, out cost, out fail)
                    && Services.Get<BeeSystem>().SpawnBee(selectedScene, hive, out fail, true)
                        != null
                )
                {
                    GD.Print($"Spawning {selectedType.Name}");
                    placementParticles.Emit(grid.GridToWorld(pos));
                    ChargeHoney(selectedType, cost);
                    success = true;
                }
                break;

            case Mode.RemoveTile:
                if (grid.RemoveTile(pos, out fail))
                {
                    removalParticles.Emit(grid.GridToWorld(pos));
                    success = true;
                }
                break;

            case Mode.RemoveObject:
                if (grid.RemoveObject(pos, out fail))
                {
                    removalParticles.Emit(grid.GridToWorld(pos));
                    success = true;
                }
                break;

            case Mode.RemoveBee:
                if (highlighted is Hive targetHive && removeBeeResource != null)
                {
                    string beeType = removeBeeResource.BeeTypeName;
                    Type t = beeType switch
                    {
                        "Base" => typeof(BaseBee),
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
                    else
                        success = true;
                }
                break;
        }

        var audio = Services.Get<AudioSystem>();
        if (fail != null)
        {
            GD.Print($"[PlacementSystem] {fail.Log}");
            Services.Get<HoverLabel>().ShowError(fail);
            audio.PlaySound("error");
        }
        else if (success && PlaceSounds.TryGetValue(CurMode, out var sound))
        {
            audio.PlaySound(sound);
        }
    }
}
