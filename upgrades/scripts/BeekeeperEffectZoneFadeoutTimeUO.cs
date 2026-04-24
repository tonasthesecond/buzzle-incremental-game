using Godot;

[GlobalClass]
public partial class BeekeeperEffectZoneFadeoutTimeUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    private float value => GameStore.BeekeeperEffectZoneFadeoutTime.Value;

    public override string GetTechnicalText() =>
        $"{Style.CK("Beekeeper aura", "noun_beekeeper")} lingers for {Style.NC(value, value + IncreaseBy, showChange: !IsMaxLevel())}s after leaving";

    public override void Apply() => GameStore.BeekeeperEffectZoneFadeoutTime.AddFlat(Name, IncreaseBy * Level);
}
