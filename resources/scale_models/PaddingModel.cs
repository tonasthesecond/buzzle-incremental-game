using Godot;

[GlobalClass]
public partial class PaddingModel : IScaleModel
{
    [Export]
    public float[] Padding { get; set; } = new float[] { 0f, 0f };

    [Export]
    public IScaleModel Model { get; set; } = null!;

    public override float Get(int x)
    {
        if (x < Padding.Length)
            return Padding[x];
        return Model.Get(x - Padding.Length);
    }

    public PaddingModel() { }

    public PaddingModel(float[] padding, IScaleModel model)
    {
        Padding = padding;
        Model = model;
    }
}
