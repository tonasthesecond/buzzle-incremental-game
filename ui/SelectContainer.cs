using Godot;
using Godot.Collections;

[GlobalClass]
public partial class SelectContainer : PanelContainer
{
    [Export]
    public Array<SelectedResource> SelectedResources { get; set; }

    [Export]
    public GrowDirection ExpandDirection { get; set; } = GrowDirection.Begin;

    [Export]
    public float MaxWidth { get; set; } = 200f;

    [Export]
    public float TweenDuration { get; set; } = 0.2f;

    private ButtonGroup buttonGroup = GD.Load<ButtonGroup>(
        "res://resources/global_button_group.tres"
    );

    private PackedScene SelectableScene = GD.Load<PackedScene>("res://ui/Selectable.tscn");

    private MarginContainer contentsContainer = null!;
    private Container selectablesContainer = null!;
    private Container expandContainer = null!;
    private Button expandButton = null!;

    public int SelectedIndex = -1;
    public bool IsExpanded = false;

    public override void _Ready()
    {
        GrowHorizontal = ExpandDirection;

        // get nodes
        contentsContainer = GetNode<MarginContainer>("%ContentsContainer");
        selectablesContainer = GetNode<Container>("%SelectablesContainer");
        expandContainer = GetNode<Container>("%ExpandContainer");
        expandButton = GetNode<Button>("%ExpandButton");

        // clear templates
        foreach (Node child in selectablesContainer.GetChildren())
            child.QueueFree();

        // add resources
        foreach (SelectedResource selectedResource in SelectedResources)
            Add(selectedResource);

        // connect signals
        expandButton.Pressed += () =>
        {
            if (IsExpanded)
                Shrink();
            else
                Expand();
        };
    }

    /// Input handling.
    public override void _UnhandledInput(InputEvent e)
    {
        /// Unpressing when right clicking.
        if (e is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Right)
            Reset();
    }

    public void Add(SelectedResource selectedResource, int index = -1)
    {
        if (index == -1)
            index = SelectedResources.Count;
        SelectedResources.Add(selectedResource);
        var selectable = SelectableScene.Instantiate<Selectable>();
        selectablesContainer.AddChild(selectable);
        selectable.Setup(SelectedResources.Count, buttonGroup, selectedResource.ImagePath);
        selectable.Selected += (int index) =>
        {
            SelectedIndex = index;
        };
    }

    /// Reset the button states.
    public void Reset()
    {
        SelectedIndex = -1;
        foreach (Selectable selectable in selectablesContainer.GetChildren())
        {
            selectable.Button.ButtonPressed = false;
        }
    }

    /// Return the selected resource.
    public Resource? GetSelectedResource() =>
        SelectedIndex == -1 ? null : SelectedResources[SelectedIndex].Resource;

    /// --- Expand/Shrink ---
    public void Expand()
    {
        TweenWidth(
            float.Min(MaxWidth, contentsContainer.Size.X)
                + contentsContainer.GetThemeConstant("margin_right") / 4
        );
        IsExpanded = true;
    }

    public void Shrink()
    {
        TweenWidth(0f);
        IsExpanded = false;
    }

    private void TweenWidth(float target)
    {
        var tween = CreateTween();
        tween.TweenProperty(expandContainer, "custom_minimum_size:x", target, TweenDuration);
    }
}
