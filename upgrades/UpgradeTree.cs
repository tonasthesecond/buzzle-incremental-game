using System;
using System.Linq;
using Godot;

public partial class UpgradeTree : Control
{
    public override void _EnterTree()
    {
        Services.Register(this);
    }

    private const string UpgradePath = "res://upgrades/resources/";

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
