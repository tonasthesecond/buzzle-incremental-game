using Godot;

[GlobalClass]
public partial class Sunflower : Flower
{
    public override Stat HoneyCost { set; get; } = new(() => GameStore.SunflowerHoneyCost.Value);
    public override Stat HoneyGain { set; get; } = new(() => GameStore.SunflowerHoneyGain.Value);
    public override Stat PollinationTime { set; get; } =
        new(() => GameStore.SunflowerPollinationTime.Value);

    const string GigantismKey = "gigantism";

    public Sunflower()
    {
        GameStore.SunflowerHoneyGainPerFatBeeBonus.Changed += UpdateGigantism;
        Services.Get<BeeSystem>().BeeSpawned += (Bee b) =>
        {
            if (b is FatBee)
                UpdateGigantism();
        };
    }

    private void UpdateGigantism()
    {
        HoneyGain.Remove(GigantismKey);
        HoneyGain.AddFlat(GigantismKey, GameStore.SunflowerHoneyGainPerFatBeeBonus.Value);
    }

    protected sealed override string GetTechnicalText()
    {
        string desc = base.GetTechnicalText() + "\n";
        if (GameStore.SunflowerHoneyGainPerFatBeeBonus.Value > 0)
            if (Placed)
            {
                int fatBeeCount = Services.Get<BeeSystem>().GetBeeCountOfType(typeof(FatBee));
                desc +=
                    $"\n{Style.CK("Gigantism Bonus")}: +{Style.CK((GameStore.SunflowerHoneyGainPerFatBeeBonus.Value * fatBeeCount).ToString("F0"))} honey ({fatBeeCount} {Style.CK("Fat bees", "noun_fat")})";
            }
            else
            {
                desc +=
                    $"\n{Style.CK("Gigantism Bonus")}: +{Style.CK((GameStore.SunflowerHoneyGainPerFatBeeBonus.Value).ToString("F0"))} honey per {Style.CK("Fat bee", "noun_fat")}";
            }
        return desc;
    }
}
