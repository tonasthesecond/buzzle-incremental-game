using System;
using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class UpgradeNode
    : Node2D,
        IHasHoverTitle,
        IHasHoverDescription,
        IHasHoverSubtitle,
        IHasHoverPrice,
        IHasHoverRefresh
{
    // soft cast for editor, otherwise it bugs out with "Cannot convert from 'Resource' to 'IUpgradeOption'"
    [Export]
    public Resource UpgradeResource { get; set; }
    public IUpgradeOption? Upgrade => UpgradeResource as IUpgradeOption;

    [Export]
    public Texture2D Icon { get; set; }

    // dependencies: upgrade node, level
    [Export]
    public Dictionary<NodePath, int> Dependencies;

    public bool IsShown { get; set; } = true; // default true for editor
    private BaseButton button = null!;

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
                GD.Print($"[UpgradeNode] {failMessage.Log}");
            }
        }
    }

    /// Check if all dependencies are met.
    private bool isDependencyMet(out FailMessage? failMessage)
    {
        foreach (NodePath path in Dependencies.Keys)
        {
            var node = GetNode<UpgradeNode>(path);
            if (node.Upgrade == null)
                continue;
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

    public string GetHoverTitle() => Upgrade?.Name ?? "";

    public string GetHoverDescription() => Upgrade?.GetHoverDescription() ?? "";

    public string GetHoverSubtitle()
    {
        string max = Upgrade?.MaxLevel != -1 ? Upgrade!.MaxLevel.ToString() : "inf";
        return $"lvl. {Upgrade?.Level}/{max}";
    }

    public int GetHoverCost()
    {
        if (Upgrade == null)
            return 0;
        if (Upgrade.Level < Upgrade.MaxLevel)
            return Upgrade.GetCost();
        return 0;
    }

    public bool IsEnough() => Upgrade?.IsEnough() ?? false;

    public void RegisterRefresh(Action onRefresh) =>
        Upgrade!.Applied += new IUpgradeOption.AppliedEventHandler(onRefresh);

    public void UnregisterRefresh(Action onRefresh) =>
        Upgrade!.Applied -= new IUpgradeOption.AppliedEventHandler(onRefresh);
}
