using Godot;

[GlobalClass]
public partial class Yarrow : Flower
{
    public override Stat HoneyCost { set; get; } = new(() => GameStore.YarrowHoneyCost.Value);
    public override Stat HoneyGain { set; get; } = new(() => GameStore.YarrowHoneyGain.Value);
    public override Stat PollinationTime { set; get; } =
        new(() => GameStore.YarrowPollinationTime.Value);
}
