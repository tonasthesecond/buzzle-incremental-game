using Godot;

/// base * (x + 1)^exp
[GlobalClass]
public partial class PolynomialModel : IScaleModel
{
    [Export]
    public float Base { get; set; } = 10f;

    [Export]
    public float Exponent { get; set; } = 2f;

    public override float Get(int x) => Base * Mathf.Pow(x + 1, Exponent);

    public PolynomialModel() { }

    public PolynomialModel(float baseValue, float exponent)
    {
        Base = baseValue;
        Exponent = exponent;
    }
}
