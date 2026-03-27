using Godot;

public partial class GeneralDescriptionUI : PanelContainer
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

    public void Setup(Node target)
    {
        switch (target)
        {
            case UpgradeNode upgradeNode:
                SetTitle(upgradeNode.Upgrade.Name);
                SetDescription(upgradeNode.Upgrade.GetText());
                upgradeNode.Upgrade.Applied += () => Setup(target);
                break;
            case BaseGridObject obj:
                SetTitle(obj.ObjectName);
                SetDescription(obj.Description);
                break;
        }
    }
}
