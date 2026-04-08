using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class HoverPointer : Control
{
    [Export]
    private Vector2 offset = new(16, 16);

    private PackedScene TDScene = GD.Load<PackedScene>("uid://caeokvlcu74wj"); // title-description
    private PackedScene TSDScene = GD.Load<PackedScene>("uid://0n7bdgefraxr"); // title-subtitle-description
    private PackedScene TSPDScene = GD.Load<PackedScene>("uid://b7j8yd82762kx"); // title-subtitle-price-description

    private Dictionary<PackedScene, Type[]> scenesUI;

    public HoverPointer()
    {
        scenesUI = new()
        {
            { TDScene, new Type[] { typeof(HiveGridObject) } },
            { TSDScene, new Type[] { typeof(HiveGridObject), typeof(Flower) } },
            { TSPDScene, new Type[] { typeof(UpgradeNode), typeof(Selectable) } },
        };
    }

    /// Spawn the appropriate UI for the hovered node.
    private void OnHovered(Node target)
    {
        Clear();
        foreach ((PackedScene scene, Type[] types) in scenesUI)
        {
            if (types.Any(type => type.IsInstanceOfType(target)))
                Spawn(scene, target);
        }
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
