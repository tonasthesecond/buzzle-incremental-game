using Godot;

[GlobalClass]
public partial class Rose : Flower
{
    public override Stat HoneyCost { set; get; } = new(() => GameStore.RoseHoneyCost.Value);
    public override Stat HoneyGain { set; get; } = new(() => GameStore.RoseHoneyGain.Value);
    public override Stat PollinationTime { set; get; } =
        new(() => GameStore.RosePollinationTime.Value);
}
