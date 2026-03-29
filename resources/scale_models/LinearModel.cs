using Godot;

/// base + x * step
[GlobalClass]
public partial class LinearModel : IScaleModel
{
    [Export]
    public float Base { get; set; } = 1f;

    [Export]
    public float Step { get; set; } = 1f;

    public override float Get(int x) => Base + Step * x;
}
