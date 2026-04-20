using System.Linq;
using Godot;

[Tool]
public partial class UpgradeTree : Control
{
    [Export]
    public bool ShowAllUpgrades = false;

    private UpgradeNode[] nodes => GetChildren().OfType<UpgradeNode>().ToArray();

    public override void _EnterTree()
    {
        if (Engine.IsEditorHint())
            return;
        Services.Register(this);
    }

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            foreach (var node in nodes)
            {
                if (node.Upgrade == null)
                    continue;
                node.Upgrade.Applied += () => QueueRedraw();
            }
            return;
        }
        foreach (UpgradeNode node in nodes)
        {
            if (node.Upgrade == null)
                continue;
            node.Upgrade.Applied += () => RedrawLines();
        }
        SignalBus.Instance.GameLoaded += () => ApplyUpgrades();

        if (ShowAllUpgrades)
        {
            GameStore.UnlockAll();
            RedrawLines();
        }
    }

    private void RedrawLines()
    {
        // clear old lines
        foreach (var child in GetChildren().OfType<Line2D>())
            child.QueueFree();

        void DrawLine(Vector2 a, Vector2 b, float width = 2)
        {
            var line = new Line2D();
            line.AddPoint(a);
            line.AddPoint(b);
            line.DefaultColor = new Color(GameStore.Colors["upgrade_line"]);
            line.Width = width;
            line.ZIndex = -1;
            AddChild(line);
        }

        if (ShowAllUpgrades)
        {
            foreach (var node in nodes)
            {
                node.ShowNode();
                foreach (NodePath path in node.Dependencies.Keys)
                {
                    var dep = node.GetNode<UpgradeNode>(path);
                    if (!IsInstanceValid(dep) || !dep.IsShown)
                        continue;
                    DrawLine(
                        node.GlobalPosition - GlobalPosition,
                        dep.GlobalPosition - GlobalPosition
                    );
                }
            }
            return;
        }

        foreach (UpgradeNode node in nodes)
        {
            if (node.Dependencies == null || !node.IsShown)
                continue;

            foreach (NodePath path in node.Dependencies.Keys)
            {
                var dep = node.GetNode<UpgradeNode>(path);
                if (!IsInstanceValid(dep) || !dep.IsShown)
                    continue;
                DrawLine(node.GlobalPosition - GlobalPosition, dep.GlobalPosition - GlobalPosition);
            }
        }
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

    public UpgradeNode[] GetUpgradeNodes() => nodes;

    public void SaveUpgrades()
    {
        GameStore.Save.Upgrades.Clear();
        foreach (UpgradeNode node in GetUpgradeNodes())
        {
            if (node.Upgrade == null)
                continue;

            var saved = new SavedUpgrade { Id = node.Name, Level = node.Upgrade.Level };
            GameStore.Save.Upgrades.Add(saved);
        }
    }

    public void ApplyUpgrades()
    {
        foreach (SavedUpgrade saved in GameStore.Save.Upgrades)
        {
            UpgradeNode node = GetUpgradeNodes().FirstOrDefault(n => n.Name == saved.Id)!;

            if (node.Upgrade == null)
            {
                GD.PushError($"[UpgradeTree] Missing upgrade node: {saved.Id}");
                continue;
            }

            node.Upgrade.Level = saved.Level;
            for (int i = 0; i <= saved.Level; i++)
            {
                node.Upgrade.EmitSignal(IUpgradeOption.SignalName.Applied);
                node.Upgrade.Apply();
            }
        }
    }
}
