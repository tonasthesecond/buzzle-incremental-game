using Godot;

[GlobalClass]
public partial class HoverPointer : Control
{
    [Export]
    private Vector2 offset = new(4, 4);

    private PackedScene HoverUIScene = GD.Load<PackedScene>("uid://b7j8yd82762kx");

    private Vector2 cachedSize = Vector2.Zero;
    private Control? activeUI;

    public override void _EnterTree()
    {
        Services.Register(this);
    }

    public override void _Ready()
    {
        SignalBus.Instance.Hovered += OnHovered;
        SignalBus.Instance.Unhovered += Clear;
    }

    public override void _Process(double delta)
    {
        var mouse = GetGlobalMousePosition();
        var screenMouse = GetViewport().GetMousePosition();
        var viewport = GetViewportRect().Size;
        float x =
            screenMouse.X + cachedSize.X + offset.X > viewport.X
                ? mouse.X - offset.X - cachedSize.X
                : mouse.X + offset.X;
        float y =
            screenMouse.Y + cachedSize.Y + offset.Y > viewport.Y
                ? mouse.Y - offset.Y - cachedSize.Y
                : mouse.Y + offset.Y;
        GlobalPosition = new Vector2(x, y);
    }

    /// Spawn hover UI for the hovered node if it has anything to show.
    private void OnHovered(Node target)
    {
        Clear();
        if (target is not IHasHoverTitle)
            return;
        var ui = HoverUIScene.Instantiate<Control>();
        activeUI = ui;
        AddChild(ui);
        ui.MinimumSizeChanged += CacheSize;
        ui.Hide();
        if (ui is HoverUI hoverUI)
            hoverUI.Setup(target);
    }

    private async void CacheSize()
    {
        if (activeUI != null)
            cachedSize = activeUI.GetCombinedMinimumSize();

        await ToSignal(GetTree(), "process_frame");
        if (!IsInstanceValid(activeUI))
            return;
        activeUI.Show();
    }

    private void Clear()
    {
        foreach (Node child in GetChildren())
            child.QueueFree();
        cachedSize = Vector2.Zero;
    }
}
