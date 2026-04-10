using Godot;

[GlobalClass]
public partial class Poppy : Flower
{
    public override Stat HoneyCost { set; get; } = new(() => GameStore.PoppyHoneyCost.Value);
    public override Stat HoneyGain { set; get; } = new(() => GameStore.PoppyHoneyGain.Value);
    public override Stat PollinationTime { set; get; } =
        new(() => GameStore.PoppyPollinationTime.Value);
}
