using System;
using Godot;

[GlobalClass]
public partial class HoverUI : PanelContainer
{
    private RichTextLabel titleLabel = null!;
    private RichTextLabel descriptionLabel = null!;
    private RichTextLabel subtitleLabel = null!;
    private RichTextLabel priceLabel = null!;

    public override void _Ready()
    {
        titleLabel = GetNode<RichTextLabel>("%TitleLabel");
        descriptionLabel = GetNode<RichTextLabel>("%DescriptionLabel");
        subtitleLabel = GetNode<RichTextLabel>("%SubtitleLabel");
        priceLabel = GetNode<RichTextLabel>("%PriceLabel");
    }

    public void Setup(Node target)
    {
        if (target is IHasHoverTitle t && t.GetHoverTitle() != "")
            titleLabel.Text = Style.Title(t.GetHoverTitle());

        if (target is IHasHoverDescription d && d.GetHoverDescription() != "")
            descriptionLabel.Text = d.GetHoverDescription();

        if (target is IHasHoverSubtitle s && s.GetHoverSubtitle() != "")
        {
            subtitleLabel.Show();
            subtitleLabel.Text = Style.SubTitle(s.GetHoverSubtitle());
        }
        else
            subtitleLabel.Hide();

        if (target is IHasHoverPrice p && p.GetHoverCost() > 0)
        {
            priceLabel.Show();
            priceLabel.Text = Style.Price(p.GetHoverCost(), p.IsEnough());
            GameStore.Instance.HoneyChanged += (_) =>
            {
                if (!IsInstanceValid(this))
                    return;
                priceLabel.Text = Style.Price(p.GetHoverCost(), p.IsEnough());
            };
        }
        else
            priceLabel.Hide();

        if (target is IHasHoverRefresh r)
        {
            Action onRefresh = null!;
            onRefresh = () =>
            {
                if (!IsInstanceValid(this))
                {
                    r.UnregisterRefresh(onRefresh);
                    return;
                }
                Setup(target);
            };
            r.RegisterRefresh(onRefresh);
        }
    }
}
