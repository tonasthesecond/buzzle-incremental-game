using Godot;
using Godot.Collections;

/// explicit value per level, clamps to last entry
[GlobalClass]
public partial class StepsModel : IScaleModel
{
    [Export]
    public Array<float> Values { get; set; } = [10f, 20f, 40f];

    public override float Get(int x) => Values[Mathf.Min(x, Values.Count - 1)];

    public StepsModel() { }

    public StepsModel(Array<float> values)
    {
        Values = values;
    }
}
