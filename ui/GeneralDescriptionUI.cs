using Godot;

public partial class GeneralDescriptionUI : PanelContainer, IHoverUI
{
    private RichTextLabel titleLabel = null!;
    private RichTextLabel descriptionLabel = null!;

    public override void _Ready()
    {
        titleLabel = GetNode<RichTextLabel>("%TitleLabel");
        descriptionLabel = GetNode<RichTextLabel>("%DescriptionLabel");
    }

    public void SetTitle(string title)
    {
        titleLabel.Text = $"[b]{title}[/b]";
    }

    public void SetDescription(string description)
    {
        descriptionLabel.Text = description;
    }

    // required
    public void Setup(Node target)
    {
        switch (target)
        {
            case UpgradeNode upgradeNode:
                SetTitle(upgradeNode.Upgrade.Name);
                SetDescription(upgradeNode.Upgrade.GetText());
                // refresh if upgrade is applied while the ui is open
                upgradeNode.Upgrade.Applied += () =>
                {
                    if (!IsInstanceValid(this))
                        return;
                    Setup(target);
                };
                break;
            case BaseGridObject obj:
                SetTitle(obj.ObjectName);
                SetDescription(obj.Description);
                break;
        }
    }
}
