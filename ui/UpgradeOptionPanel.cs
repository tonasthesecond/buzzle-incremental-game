using System;
using Godot;

public partial class UpgradeOptionPanel : PanelContainer
{
    [Signal]
    public delegate void ButtonPressedEventHandler();
    private RichTextLabel descriptionLabel;
    private Label priceLabel;
    private Button buyButton;

    public override void _Ready()
    {
        descriptionLabel = GetNode<RichTextLabel>("%DescriptionLabel");
        priceLabel = GetNode<Label>("%PriceLabel");
        buyButton = GetNode<Button>("%BuyButton");
        buyButton.Pressed += () => EmitSignal(SignalName.ButtonPressed);
    }

    public void Setup(string description, string price, string buyButtonText)
    {
        SetDescription(description);
        SetPrice(price);
        SetBuyButtonText(buyButtonText);
    }

    public void SetDescription(string description)
    {
        descriptionLabel.Text = description;
    }

    public void SetPrice(string price)
    {
        priceLabel.Text = price;
    }

    public void SetBuyButtonText(string buyButtonText)
    {
        buyButton.Text = buyButtonText;
    }
}
