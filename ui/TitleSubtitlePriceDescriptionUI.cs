using Godot;

public partial class TitleSubtitlePriceDescriptionUI : TitleDescriptionUI
{
    protected RichTextLabel subtitleLabel = null!;
    protected RichTextLabel priceLabel = null!;

    public override void _Ready()
    {
        base._Ready();
        subtitleLabel = GetNode<RichTextLabel>("%SubtitleLabel");
        priceLabel = GetNode<RichTextLabel>("%PriceLabel");
    }

    public void SetLevel(string level, string maxLevel)
    {
        subtitleLabel.Text = Style.SubTitle($"lvl. {level}/{maxLevel}");
    }

    public void SetCount(string count)
    {
        subtitleLabel.Text = Style.SubTitle($"count: {count}");
    }

    public void SetPrice(int price, bool isEnough = true)
    {
        priceLabel.Text = Style.Price(price, isEnough);
    }

    public override void Setup(Node target)
    {
        switch (target)
        {
            case UpgradeNode upgradeNode:
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

                // update price if honey changes
                GameStore.Instance.HoneyChanged += (_) =>
                {
                    if (!IsInstanceValid(this))
                        return;
                    SetPrice(upgrade.GetCost(), upgrade.IsEnough());
                };
                break;

            case Selectable selectable:
                if (selectable.Resource.ResourceName == "RemoveTile")
                {
                    SetTitle("Remove a Tile");
                    priceLabel.Hide();
                    SetDescription("Remove a tile from the grid.");
                    return;
                }
                else if (selectable.Resource.ResourceName == "RemoveObject")
                {
                    SetTitle("Remove an Object");
                    priceLabel.Hide();
                    SetDescription("Remove an object from the grid.");
                    return;
                }
                if (selectable.Resource.Resource is not PackedScene scene)
                    return;
                Node node = scene.Instantiate();
                base.Setup(node);
                switch (node)
                {
                    case HiveGridObject hiveGridObject:
                        subtitleLabel.Hide();
                        priceLabel.Hide();
                        break;
                }
                break;
        }
    }
}
