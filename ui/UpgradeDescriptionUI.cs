using Godot;

public partial class UpgradeDescriptionUI : GeneralDescriptionUI
{
    protected RichTextLabel levelLabel = null!;
    protected RichTextLabel priceLabel = null!;

    public override void _Ready()
    {
        base._Ready();
        levelLabel = GetNode<RichTextLabel>("%LevelLabel");
        priceLabel = GetNode<RichTextLabel>("%PriceLabel");
    }

    public void SetLevel(string level, string maxLevel)
    {
        levelLabel.Text = Style.SubTitle($"lvl. {level}/{maxLevel}");
    }

    public void SetPrice(int price, bool isEnough = true)
    {
        priceLabel.Text = Style.Price(price, isEnough);
    }

    public override void Setup(Node target)
    {
        if (target is not UpgradeNode upgradeNode)
            return;

        IUpgradeOption? upgrade = upgradeNode.Upgrade;
        if (upgrade == null)
            return;
        SetTitle(upgrade.Name);
        SetDescription(upgrade.GetText());

        // refresh if upgrade is applied while the ui is open.
        IUpgradeOption.AppliedEventHandler onApplied = null!;
        onApplied = () =>
        {
            if (!IsInstanceValid(this))
            {
                upgrade.Applied -= onApplied; // remove event handler
                return;
            }
            Setup(target);
        };
        upgrade.Applied += onApplied;

        SetLevel(
            upgrade.Level.ToString(),
            upgrade.MaxLevel != -1 ? upgrade.MaxLevel.ToString() : "inf"
        );

        // hide price if upgrade is max level
        if (upgrade.Level == upgrade.MaxLevel)
            priceLabel.Hide();
        else
            SetPrice(upgrade.GetCost(), upgrade.IsEnough());

        GameStore.Instance.HoneyChanged += (_) =>
        {
            if (!IsInstanceValid(this))
                return;
            SetPrice(upgrade.GetCost(), upgrade.IsEnough());
        };
    }
}
