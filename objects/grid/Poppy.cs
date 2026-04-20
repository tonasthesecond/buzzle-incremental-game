using Godot;

[GlobalClass]
public partial class Poppy : Flower
{
    public override Stat HoneyCost { set; get; } = new(() => GameStore.PoppyHoneyCost.Value);
    public override Stat HoneyGain { set; get; } = new(() => GameStore.PoppyHoneyGain.Value);
    public override Stat PollinationTime { set; get; } =
        new(() => GameStore.PoppyPollinationTime.Value);

    public override string GetHoverDescription()
    {
        string desc =
            $" This {Style.CK("flower", "noun_flower")} costs little, produces little, and quick to pollinate.\n\n";

        desc +=
            $"{Style.CK("Production", "noun_poppy")}: {Style.CK(HoneyGain.Value.ToString("F0"))} honey\n";
        desc +=
            $"{Style.CK("Cost", "noun_poppy")}: {Style.CK(HoneyCost.Value.ToString("F0"))} honey\n";
        desc +=
            $"{Style.CK("Pol. Time", "noun_poppy")}: {Style.CK(PollinationTime.Value.ToString("F1"))} seconds";

        return desc;
    }
}
