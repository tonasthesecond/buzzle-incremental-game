using Godot;

[GlobalClass]
public partial class BeekeeperEffectZoneSpeedBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.1f;

    private float value => GameStore.BeekeeperEffectZoneSpeedBuff.Value;

    public override string GetHoverDescription() => 
        $"{Style.CK("Beekeeper Effect Zone", "noun_beekeeper")} increases all {Style.CK("Bees", "noun_bee")}' speed by {Style.NumberChangePercent(value, value + IncreaseBy)}";

    public override void Apply()
    {
        GameStore.BeekeeperEffectZoneSpeedBuff.AddFlat(Name, IncreaseBy * Level);
    }
}
