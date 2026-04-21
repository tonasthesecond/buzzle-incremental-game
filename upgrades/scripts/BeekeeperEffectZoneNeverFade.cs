using Godot;

[GlobalClass]
public partial class BeekeeperEffectZoneNeverFade : IUpgradeOption
{
    public override string GetTechnicalText() =>
        $"{Style.CK("Beekeeper Effect Zone", "noun_beekeeper")} lingers forever.";

    public override void Apply()
    {
        if (Level > 0)
            GameStore.BeekeeperEffectZoneNeverFade = true;
    }
}
