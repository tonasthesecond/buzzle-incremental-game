using Godot;
using Godot.Collections;

[GlobalClass]
[Tool]
public partial class UpgradeNode : Node2D
{
    // soft cast for editor, otherwise it bugs out because UpgradeOption is abstract
    [Export]
    public Resource UpgradeResource { get; set; }
    public UpgradeOption? Upgrade => UpgradeResource as UpgradeOption;

    [Export]
    public Texture2D Icon { get; set; }
    public bool IsShown { get; set; } = false;

    // dependencies: upgrade node, level
    [Export]
    public Dictionary<NodePath, int> Dependencies;

    public override void _Ready()
    {
        GetNode<TextureRect>("%IconRect").Texture = Icon;
        if (Engine.IsEditorHint())
            return;
        if (Dependencies == null)
        {
            IsShown = true;
            return;
        }

        // connect signals
        foreach (NodePath path in Dependencies.Keys)
        {
            var node = GetNode<UpgradeNode>(path);
            if (!IsInstanceValid(node) || node.Upgrade == null)
                continue;
            node.Upgrade.Applied += OnDependencyApplied;
        }
    }

    public override void _Draw()
    {
        // draw dependency lines
        if (Dependencies == null)
            return;
        foreach (NodePath path in Dependencies.Keys)
        {
            var node = GetNode<UpgradeNode>(path);
            if (!IsInstanceValid(node))
                continue;
            DrawLine(ToLocal(node.GlobalPosition), Vector2.Zero, Colors.LightGray, 2);
        }
    }

    public void OnDependencyApplied()
    {
        bool draw = true;
        foreach (NodePath path in Dependencies.Keys)
        {
            var node = GetNode<UpgradeNode>(path);
            if (node.Upgrade.Level < Dependencies[path] || !node.IsShown)
                draw = false;
        }
        if (draw)
            ShowNode();
        else
            HideNode();
    }

    public void ShowNode()
    {
        IsShown = true;
        Show();
    }

    public void HideNode()
    {
        IsShown = false;
        Hide();
    }
}
