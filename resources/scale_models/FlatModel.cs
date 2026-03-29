using Godot;

/// flat value regardless of x
[GlobalClass]
public partial class FlatModel : IScaleModel
{
    [Export]
    public float Value { get; set; } = 10f;

    public override float Get(int x) => Value;
}
