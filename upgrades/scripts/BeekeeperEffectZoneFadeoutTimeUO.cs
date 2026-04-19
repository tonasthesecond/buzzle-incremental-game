using Godot;

[GlobalClass]
public partial class BeekeeperEffectZoneFadeoutTimeUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    private float value => GameStore.BeekeeperEffectZoneFadeoutTime.Value;

    public override string GetTechnicalText() => 
        $"{Style.CK("Beekeeper Effect Zone", "noun_beekeeper")} lingers for {Style.NumberChange(value, value + IncreaseBy)} seconds.";

    public override void Apply()
    {
        GameStore.BeekeeperEffectZoneFadeoutTime.AddFlat(Name, IncreaseBy * Level);
    }
}
