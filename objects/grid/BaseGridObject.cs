using Godot;

[GlobalClass]
public partial class BaseGridObject : Node2D, IHasHoverTitle, IHasHoverDescription, IHasHoverPrice
{
    [Export]
    public string ObjectName { get; set; } = "<Object>";

    public Vector2I GridPosition { get; set; }

    protected AnimatedSprite2D sprite = null!;
    private HoverAreaComponent hoverArea = null!;
    private float hoverDelay { get; set; } = 0.5f;

    public bool Placed { get; set; } = false;

    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite2D>("%Sprite");
        hoverArea = GetNode<HoverAreaComponent>("%HoverAreaComponent");
        hoverArea.Setup(this, hoverDelay);
    }

    public string GetHoverTitle()
    {
        return ObjectName;
    }

    public virtual string GetHoverDescription()
    {
        if (!Placed)
            return "";

        return $"{ObjectName} at ({GridPosition.X}, {GridPosition.Y})";
    }

    public virtual int GetHoverCost()
    {
        if (!Placed)
            return GameStore.GetPlacementCost(GetType());
        return 0;
    }

    public virtual bool IsEnough() => GetHoverCost() <= GameStore.Honey;
}
