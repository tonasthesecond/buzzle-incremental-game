using Godot;

[GlobalClass]
public partial class BeekeeperFadeoutTimeUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    private float value => GameStore.BeekeeperEffectZoneFadeoutTime.Value;

    public override string GetTechnicalText() =>
        $"{Style.CK("Beekeeper aura", "noun_beekeeper")} fadeout time {Style.NC(value, value + IncreaseBy, showChange: !IsMaxLevel())}s";

    public override void Apply() => GameStore.BeekeeperEffectZoneFadeoutTime.AddFlat(Name, IncreaseBy * Level);
}
