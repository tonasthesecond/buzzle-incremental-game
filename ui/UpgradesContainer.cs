using Godot;

public partial class UpgradesContainer : PanelContainer
{
    [Export]
    public required Godot.Collections.Array<UpgradeOption> UpgradeOptions;

    private PackedScene upgradePanelScene = GD.Load<PackedScene>("uid://ck8u8w51dthbx");

    private Container optionsContainer = null!;

    public override void _Ready()
    {
        optionsContainer = GetNode<Container>("%OptionsContainer");

        // connect signals
        GameStore.Instance.OnHoneyChanged += (_) => UpdatePanels();

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
        // update colors
        string priceTextColor;
        if (GameStore.Honey < uo.GetCost())
        {
            priceTextColor = GameStore.Colors["price_not_enough"];
        }
        else
        {
            priceTextColor = GameStore.Colors["price_enough"];
        }
        panel.Setup(uo.GetText(), uo.GetCost().ToString(), " Buy ");
        panel.PriceLabel.Modulate = Color.FromString(priceTextColor, Colors.White);
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
