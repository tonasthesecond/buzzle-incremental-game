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

    public void Setup(UpgradeNode target)
    {
        SetTitle(target.Upgrade.Name);
        SetDescription(target.Upgrade.GetText());
        target.Upgrade.Applied += () => Setup(target);
    }
}
