using System;
using Godot;

[GlobalClass]
public partial class GeneralDescriptionUI : PanelContainer, IHoverUI
{
    protected RichTextLabel titleLabel = null!;
    protected RichTextLabel descriptionLabel = null!;

    public override void _Ready()
    {
        titleLabel = GetNode<RichTextLabel>("%TitleLabel");
        descriptionLabel = GetNode<RichTextLabel>("%DescriptionLabel");
    }

    public void SetTitle(string title)
    {
        titleLabel.Text = Style.Title(title);
    }

    public void SetDescription(string description)
    {
        descriptionLabel.Text = description;
    }

    public virtual void Setup(Node target)
    {
        switch (target)
        {
            case UpgradeNode upgradeNode:
                IUpgradeOption? upgrade = upgradeNode.Upgrade;
                if (upgrade == null)
                    return;

                SetTitle(upgrade.Name);
                SetDescription(upgrade.GetText());
                // refresh if upgrade is applied while the ui is open
                upgrade.Applied += () =>
                {
                    if (!IsInstanceValid(this))
                        return;
                    Setup(target);
                };
                break;

            case HiveGridObject hive:
                Action setTitle = null!;
                setTitle = () =>
                {
                    if (!IsInstanceValid(this))
                    {
                        hive.BeeAdded -= (Bee bee) => setTitle();
                        return;
                    }
                    SetTitle(
                        hive.ObjectName + $" ({hive.BeeCount}/{GameStore.HiveCapacityBee.Value})"
                    );
                };
                hive.BeeAdded += (Bee bee) => setTitle();
                setTitle();
                SetDescription(hive.Description);
                break;

            case BaseGridObject obj:
                SetTitle(obj.ObjectName);
                SetDescription(obj.Description);
                break;
        }
    }
}
