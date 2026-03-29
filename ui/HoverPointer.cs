using Godot;

[GlobalClass]
public partial class HoverPointer : Control
{
    [Export]
    private Vector2 offset = new(16, 16);

    private PackedScene generalDescriptionScene = GD.Load<PackedScene>("uid://caeokvlcu74wj");
    private PackedScene upgradeDescriptionScene = GD.Load<PackedScene>("uid://b7j8yd82762kx");

    /// Spawn the appropriate UI for the hovered node.
    private void OnHovered(Node target)
    {
        Clear();
        Control? ui = target switch
        {
            UpgradeNode upgradeNode => Spawn(upgradeDescriptionScene, target),
            _ => Spawn(generalDescriptionScene, target),
        };
    }

    public override void _EnterTree()
    {
        Services.Register(this);
    }

    public override void _Ready()
    {
        Services.Register(this);
        SignalBus.Instance.Hovered += OnHovered;
        SignalBus.Instance.Unhovered += Clear;
    }

    public override void _Process(double delta)
    {
        GlobalPosition = GetGlobalMousePosition() + offset;
    }

    private Control? Spawn(PackedScene? scene, Node target)
    {
        if (scene == null)
            return null;
        var ui = scene.Instantiate<Control>();
        AddChild(ui);
        if (ui is IHoverUI hoverUI)
            hoverUI.Setup(target);
        return ui;
    }

    private void Clear()
    {
        foreach (Node child in GetChildren())
            child.QueueFree();
    }
}
