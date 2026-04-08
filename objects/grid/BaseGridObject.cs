using Godot;

[GlobalClass]
public partial class BaseGridObject : Node2D
{
    [Export]
    public string ObjectName { get; set; } = "<Object>";

    [Export]
    public string Description { get; set; } = "<ObjectDescription>";

    public Vector2I GridPosition { get; set; }

    protected AnimatedSprite2D sprite = null!;
    private HoverAreaComponent hoverArea = null!;
    private float hoverDelay { get; set; } = 0.5f;

    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite2D>("%Sprite");
        hoverArea = GetNode<HoverAreaComponent>("%HoverAreaComponent");
        hoverArea.Setup(this, hoverDelay);
    }
}
