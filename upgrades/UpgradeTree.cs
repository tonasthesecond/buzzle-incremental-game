using System;
using System.Linq;
using Godot;

[Tool]
public partial class UpgradeTree : Control
{
    private const string UpgradePath = "res://upgrades/resources/";
    private UpgradeNode[] nodes => GetChildren().OfType<UpgradeNode>().ToArray();

    public override void _EnterTree()
    {
        if (Engine.IsEditorHint())
            return;
        Services.Register(this);
    }

    public override void _Ready()
    {
        foreach (UpgradeNode node in nodes)
            node.Upgrade.Applied += () => QueueRedraw();
    }

    public override void _Draw()
    {
        foreach (UpgradeNode node in nodes)
        {
            if (node.Dependencies == null || !node.IsShown)
                continue;

            foreach (NodePath path in node.Dependencies.Keys)
            {
                var dep = node.GetNode<UpgradeNode>(path);

                if (!Engine.IsEditorHint() && (!IsInstanceValid(dep) || !dep.IsShown))
                    continue;

                DrawLine(
                    node.GlobalPosition - GlobalPosition,
                    dep.GlobalPosition - GlobalPosition,
                    Colors.LightGray,
                    2
                );
            }
        }
    }

    public UpgradeOption[] GetUpgrades()
    {
        using var dir = DirAccess.Open(UpgradePath);
        if (dir == null)
        {
            GD.PrintErr($"Could not open path: {UpgradePath}");
            return Array.Empty<UpgradeOption>();
        }
        return dir.GetFiles()
            .Where(file => file.EndsWith(".tres"))
            .Select(file => GD.Load<UpgradeOption>(UpgradePath + file))
            .Where(upgrade => upgrade != null)
            .ToArray();
    }
}
