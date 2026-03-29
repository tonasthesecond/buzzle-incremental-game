using Godot;

/// base * rate^x
[GlobalClass]
public partial class ExponentialModel : IScaleModel
{
    [Export]
    public float Base { get; set; } = 10f;

    [Export]
    public float Rate { get; set; } = 1.5f;

    public override float Get(int x) => Base * Mathf.Pow(Rate, x);
}
