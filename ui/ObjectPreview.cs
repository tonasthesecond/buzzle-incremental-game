using Godot;

public partial class ObjectPreview : TextureRect
{
    private PlacementSystem.Mode mode = PlacementSystem.Mode.None;

    public override void _Ready()
    {
        Hide();
        MouseFilter = MouseFilterEnum.Ignore;

        SignalBus.Instance.SelectedResourceSelected += OnSelectedResourceSelected;
        SignalBus.Instance.ResourceUnselected += Hide;
        Services.Get<PlacementSystem>().ModeChanged += OnModeChanged;
    }

    public override void _ExitTree()
    {
        if (!IsInstanceValid(SignalBus.Instance))
            return;
        SignalBus.Instance.SelectedResourceSelected -= OnSelectedResourceSelected;
        SignalBus.Instance.ResourceUnselected -= Hide;
    }

    private void OnSelectedResourceSelected(SelectedResource selected)
    {
        if (mode != PlacementSystem.Mode.Object && mode != PlacementSystem.Mode.Bee)
            return;
        Texture = selected.Icon;
        Modulate = new Color(1, 1, 1, 0.6f);
        Show();
    }

    private void OnModeChanged(PlacementSystem.Mode m)
    {
        mode = m;
        if (mode != PlacementSystem.Mode.Object && mode != PlacementSystem.Mode.Bee)
            Hide();
        if (mode == PlacementSystem.Mode.Bee)
            Scale = Vector2.One;
    }

    public override void _Process(double delta)
    {
        if (!Visible)
            return;
        Camera2D cam = (Camera2D)GetViewport().GetCamera2D();
        Tilemap tilemap = Services.Get<Tilemap>()!;
        Grid grid = Services.Get<Grid>()!;
        Vector2I cell = tilemap.LocalToMap(tilemap.GetLocalMousePosition());
        Vector2 worldPos = grid.GridToWorld(cell);
        if (mode == PlacementSystem.Mode.Object)
        {
            Scale = cam.Zoom;
            GlobalPosition =
                GetViewport().GetCanvasTransform() * worldPos
                - new Vector2(GameStore.TILE_SIZE / 2f, GameStore.TILE_SIZE / 2f) * cam.Zoom;
        }
        else if (mode == PlacementSystem.Mode.Bee)
        {
            Scale = Vector2.One;
            Hive hive = grid.GetClosestObjectOfType<Hive>(worldPos)!;
            if (hive != null)
            {
                Vector2 worldMouse =
                    GetViewport().GetCanvasTransform().AffineInverse() * GetGlobalMousePosition();
                FlipH = hive.GlobalPosition.X < worldMouse.X;
            }
            GlobalPosition = GetGlobalMousePosition() - Size / 2f;
        }
    }
}
