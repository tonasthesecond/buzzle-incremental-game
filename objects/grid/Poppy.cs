using Godot;

[GlobalClass]
public partial class Poppy : Flower
{
    public override Stat HoneyCost { set; get; } = new(() => GameStore.PoppyHoneyCost.Value);
    public override Stat HoneyGain { set; get; } = new(() => GameStore.PoppyHoneyGain.Value);
    public override Stat PollinationTime { set; get; } =
        new(() => GameStore.PoppyPollinationTime.Value);

    protected override string GetTechnicalText()
    {
        string desc = base.GetTechnicalText();

        if (Placed)
            if (GameStore.DirtPoppyHoneyGainBuff.Value > 0f)
            {
                desc +=
                    $"\n{Style.CK("Natural Habitat", "noun_dirt")}: +{Style.CKPercent(GameStore.DirtPoppyHoneyGainBuff.Value)} more honey";
            }
        return desc;
    }
}
