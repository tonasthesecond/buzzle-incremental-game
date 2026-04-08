using Godot;

[GlobalClass]
public partial class BaseGridObject : Node2D, IHasHoverDescription
{
    [Export]
    public string ObjectName { get; set; } = "<Object>";

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

    public virtual string GetHoverDescription()
    {
        return $"{ObjectName} at ({GridPosition.X}, {GridPosition.Y})";
    }
}
