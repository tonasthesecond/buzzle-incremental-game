using Godot;

[GlobalClass]
public partial class BeekeeperEffectZoneSpeedBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.1f;

    private float value => GameStore.BeekeeperEffectZoneSpeedBuff.Value;

    public override string GetTechnicalText() =>
        $"{Style.CK("Beekeeper aura", "noun_beekeeper")} speeds up {Style.CK("bees", "noun_bee")} by {Style.NCPercent(value, value + IncreaseBy, !IsMaxLevel())}";

    public override void Apply() => GameStore.BeekeeperEffectZoneSpeedBuff.AddFlat(Name, IncreaseBy * Level);
}
