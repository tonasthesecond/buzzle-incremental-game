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
        base.Setup(target);
        if (target is UpgradeNode upgradeNode)
        {
            IUpgradeOption? upgrade = upgradeNode.Upgrade;
            if (upgrade == null)
                return;

            SetLevel(
                upgrade.Level.ToString(),
                upgrade.MaxLevel != -1 ? upgrade.MaxLevel.ToString() : "inf"
            );
            SetPrice(upgrade.GetCost(), upgrade.IsEnough());
            GameStore.Instance.HoneyChanged += (_) =>
            {
                if (!IsInstanceValid(this))
                    return;
                SetPrice(upgrade.GetCost(), upgrade.IsEnough());
            };
        }
    }
}
