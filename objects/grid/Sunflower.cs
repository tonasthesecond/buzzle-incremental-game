using Godot;

[GlobalClass]
public partial class Sunflower : Flower
{
    public override Stat HoneyCost { set; get; } = new(() => GameStore.SunflowerHoneyCost.Value);
    public override Stat HoneyGain { set; get; } = new(() => GameStore.SunflowerHoneyGain.Value);
    public override Stat PollinationTime { set; get; } =
        new(() => GameStore.SunflowerPollinationTime.Value);
}
