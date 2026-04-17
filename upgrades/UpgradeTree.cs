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
        {
            if (node.Upgrade == null)
                continue;
            node.Upgrade.Applied += () => QueueRedraw();
        }

        SignalBus.Instance.GameLoaded += () => ApplyUpgrades();
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

    public IUpgradeOption[] GetUpgrades()
    {
        using var dir = DirAccess.Open(UpgradePath);
        if (dir == null)
        {
            GD.PrintErr($"Could not open path: {UpgradePath}");
            return Array.Empty<IUpgradeOption>();
        }
        return dir.GetFiles()
            .Where(file => file.EndsWith(".tres"))
            .Select(file => GD.Load<IUpgradeOption>(UpgradePath + file))
            .Where(upgrade => upgrade != null)
            .ToArray();
    }

    public void SaveUpgrades()
    {
        GameStore.Save.Upgrades.Clear();
        foreach (var upgrade in GetUpgrades())
        {
            var saved = new SavedUpgrade
            {
                Id = upgrade.ResourcePath.GetFile().GetBaseName(),
                Level = upgrade.Level,
            };
            GameStore.Save.Upgrades.Add(saved);
        }
    }

    public void ApplyUpgrades()
    {
        foreach (SavedUpgrade saved in GameStore.Save.Upgrades)
        {
            var upgrade = GD.Load<IUpgradeOption>("res://upgrades/resources/" + saved.Id + ".tres");
            if (upgrade == null)
            {
                GD.PushError($"[GameStore] Missing upgrade: {saved.Id}");
                continue;
            }
            upgrade.Level = saved.Level;
            for (int i = 0; i <= saved.Level; i++)
            {
                upgrade.EmitSignal(IUpgradeOption.SignalName.Applied);
                upgrade.Apply();
            }
        }
    }
}
