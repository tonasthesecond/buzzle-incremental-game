using Godot;

[GlobalClass]
public partial class TitleDescriptionUI : PanelContainer, IHoverUI
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
            case HiveGridObject hive:
                SetTitle(hive.ObjectName);
                SetDescription(hive.GetHoverDescription());
                break;

            case BaseGridObject obj:
                SetTitle(obj.ObjectName);
                SetDescription(obj.GetHoverDescription());
                break;
        }
    }
}
