using Godot;

[GlobalClass]
public partial class BaseFlower : Flower
{
    public override Stat HoneyCost { set; get; } = new(() => GameStore.BaseFlowerHoneyCost.Value);
    public override Stat HoneyGain { set; get; } = new(() => GameStore.BaseFlowerHoneyGain.Value);
    public override Stat PollinationTime { set; get; } =
        new(() => GameStore.BaseFlowerPollinationTime.Value);
}
