using System;
using Godot;

public partial class UpgradeOptionPanel : PanelContainer
{
    [Signal]
    public delegate void ButtonPressedEventHandler();
    public RichTextLabel DescriptionLabel = null!;
    public Label PriceLabel = null!;
    public Button BuyButton = null!;

    public override void _Ready()
    {
        DescriptionLabel = GetNode<RichTextLabel>("%DescriptionLabel");
        PriceLabel = GetNode<Label>("%PriceLabel");
        BuyButton = GetNode<Button>("%BuyButton");
        BuyButton.Pressed += () => EmitSignal(SignalName.ButtonPressed);
    }

    public void Setup(string description, string price, string buyButtonText)
    {
        DescriptionLabel.Text = description;
        PriceLabel.Text = price;
        BuyButton.Text = buyButtonText;
    }
}
