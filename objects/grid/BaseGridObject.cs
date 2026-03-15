using Godot;

[GlobalClass]
public partial class BaseGridObject : Node2D
{
    public Vector2I GridPosition { get; set; }

    protected AnimatedSprite2D sprite = null!;

    public override void _Ready() => sprite = GetNode<AnimatedSprite2D>("%Sprite");
}
