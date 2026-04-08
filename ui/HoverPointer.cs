using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class HoverPointer : Control
{
    [Export]
    private Vector2 offset = new(16, 16);

    private PackedScene generalDescriptionScene = GD.Load<PackedScene>("uid://caeokvlcu74wj");
    private PackedScene generalDescriptionSceneWithSubtitle = GD.Load<PackedScene>(
        "uid://0n7bdgefraxr"
    );
    private PackedScene upgradeDescriptionScene = GD.Load<PackedScene>("uid://b7j8yd82762kx");

    private Dictionary<PackedScene, Type[]> scenesUI;

    public HoverPointer()
    {
        scenesUI = new()
        {
            { generalDescriptionScene, new Type[] { typeof(HiveGridObject) } },
            {
                generalDescriptionSceneWithSubtitle,
                new Type[] { typeof(HiveGridObject), typeof(Flower) }
            },
            { upgradeDescriptionScene, new Type[] { typeof(UpgradeNode) } },
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
