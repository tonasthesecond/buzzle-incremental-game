using Godot;

[GlobalClass]
public partial class BeekeeperEffectZoneNeverFade : IUpgradeOption
{
    public override string GetTechnicalText() =>
        $"{Style.CK("Beekeeper aura", "noun_beekeeper")} never fades";

    public override void Apply()
    {
        if (Level <= 0)
            return;
        GameStore.BeekeeperEffectZoneNeverFade = true;
    }
}
