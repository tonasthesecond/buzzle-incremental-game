using Godot;

[GlobalClass]
public partial class BeekeeperEffectZoneRadiusUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 16f;

    private float value => GameStore.BeekeeperEffectZoneRadius.Value * 2;

    public override string GetTechnicalText() =>
        $"{Style.CK("Beekeeper aura", "noun_beekeeper")} covers {Style.NC(Utils.PixelsToTiles(value), Utils.PixelsToTiles(value + IncreaseBy), showChange: !IsMaxLevel())} tiles";

    public override void Apply() => GameStore.BeekeeperEffectZoneRadius.AddFlat(Name, IncreaseBy * Level);
}
