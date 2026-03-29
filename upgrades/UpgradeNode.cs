using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class UpgradeNode : Node2D
{
    // soft cast for editor, otherwise it bugs out because UpgradeOption is abstract
    [Export]
    public Resource UpgradeResource { get; set; }
    public UpgradeOption? Upgrade => UpgradeResource as UpgradeOption;

    [Export]
    public Texture2D Icon { get; set; }
    public bool IsShown { get; set; } = true; // default true for editor
    private BaseButton button = null!;

    // dependencies: upgrade node, level
    [Export]
    public Dictionary<NodePath, int> Dependencies;

    public override void _Ready()
    {
        GetNode<TextureRect>("%IconRect").Texture = Icon;
        button = GetNode<BaseButton>("%Button");
        if (Dependencies == null || Engine.IsEditorHint())
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
            node.Upgrade.Applied += onDependencyApplied;
        }
        button.Pressed += onButtonPressed;
        onDependencyApplied(); // initial check for visibility
    }

    private void onDependencyApplied()
    {
        if (isDependencyMet(out FailMessage? failMessage))
            ShowNode();
        else
            HideNode();
    }

    private void onButtonPressed()
    {
        if (isDependencyMet(out FailMessage? failMessage))
        {
            if (Upgrade.Buy(out failMessage)) { }
            else
            {
                Services.Get<ErrorLabel>().ShowError(failMessage);
                GD.Print($"[UpgradeNode] {failMessage}");
            }
        }
    }

    /// Check if all dependencies are met.
    private bool isDependencyMet(out FailMessage? failMessage)
    {
        foreach (NodePath path in Dependencies.Keys)
        {
            var node = GetNode<UpgradeNode>(path);
            if (node.Upgrade.Level < Dependencies[path] || !node.IsShown)
            {
                failMessage = new FailMessage($"Upgrade not unlocked!");
                return false;
            }
        }
        failMessage = null;
        return true;
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
