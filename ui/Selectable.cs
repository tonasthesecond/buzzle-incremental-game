using System;
using Godot;

public partial class Selectable
    : Control,
        IHasHoverTitle,
        IHasHoverDescription,
        IHasHoverSubtitle,
        IHasHoverPrice,
        IHasHoverRefresh
{
    [Signal]
    public delegate void SelectedEventHandler(int index);
    public SelectedResource Resource = null!;
    public TextureButton Button = null!;
    int Index = 0;

    public override void _Ready()
    {
        Button = GetNode<TextureButton>("%Button");

        // connect signals
        Button.Toggled += (pressed) =>
        {
            Modulate = pressed ? Colors.Green : Colors.White;
            if (pressed)
                EmitSignal(SignalName.Selected, Index);
        };
    }

    /// Set the button group for the button.
    public void Setup(int index, ButtonGroup group, Texture2D texture, SelectedResource resource)
    {
        Button.ButtonGroup = group;
        Index = index;
        Button.TextureNormal = texture;
        Button.TexturePressed = texture;
        Button.TextureHover = texture;
        Resource = resource;
    }

    public string GetHoverTitle() => Resource.GetHoverTitle();

    public string GetHoverDescription() => Resource.GetHoverDescription();

    public string GetHoverSubtitle() => Resource.GetHoverSubtitle();

    public int GetHoverCost() => Resource.GetHoverCost();

    public bool IsEnough() => Resource.IsEnough();

    public void RegisterRefresh(Action refresh) => Resource.RegisterRefresh(refresh);

    public void UnregisterRefresh(Action refresh) => Resource.UnregisterRefresh(refresh);
}
