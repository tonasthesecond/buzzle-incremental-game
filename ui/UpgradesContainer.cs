#nullable enable
using System;
using Godot;

public partial class UpgradesContainer : PanelContainer
{
    [Export]
    public Godot.Collections.Array<UpgradeOption> UpgradeOptions;

    private PackedScene upgradePanelScene = GD.Load<PackedScene>(
        "res://ui/UpgradeOptionPanel.tscn"
    );

    private Container optionsContainer;

    public override void _Ready()
    {
        optionsContainer = GetNode<Container>("%OptionsContainer");

        // free template children
        foreach (var child in optionsContainer.GetChildren())
            child.QueueFree();

        // add upgrade options
        foreach (UpgradeOption uo in UpgradeOptions)
        {
            UpgradeOptionPanel upgradePanel = upgradePanelScene.Instantiate<UpgradeOptionPanel>();
            optionsContainer.AddChild(upgradePanel);

            // setup upgrade panel and connect button
            UpdatePanel(upgradePanel, uo);
            upgradePanel.ButtonPressed += () => OnButtonPressed(uo);
        }
    }

    private void OnButtonPressed(UpgradeOption uo)
    {
        // try to buy upgrade
        if (!uo.Buy(out string? failMessage))
        {
            // TODO: implement fail message
        }
        else
            UpdatePanels();
    }

    private void UpdatePanel(UpgradeOptionPanel panel, UpgradeOption uo)
    {
        panel.Setup(uo.GetText(), $"{uo.GetCost()}", " Buy ");
    }

    private void UpdatePanels()
    {
        for (int i = 0; i < optionsContainer.GetChildCount(); i++)
        {
            UpgradeOptionPanel panel = optionsContainer.GetChild<UpgradeOptionPanel>(i);
            UpdatePanel(panel, UpgradeOptions[i]);
        }
    }
}
