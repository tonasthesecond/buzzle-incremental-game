using Godot;

[GlobalClass]
public partial class CloverJackpotChanceJackpotHoneyGainSetUO : IUpgradeOption
{
    [Export]
    public float SetChanceTo { get; set; } = 0.05f;

    [Export]
    public float SetHoneyGainTo { get; set; } = 2f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Clovers", "noun_clover")} have a {Style.NCPercent(GameStore.CloverJackpotChance.Value, SetChanceTo)} of jackpot\n{Style.CK("Clovers", "noun_clover")} produce {Style.NC(GameStore.CloverJackpotHoneyGain.Value, SetHoneyGainTo)} honey on jackpot";

    public override void Apply()
    {
        GameStore.CloverJackpotChance.RemoveAll();
        GameStore.CloverJackpotHoneyGain.RemoveAll();
        GameStore.CloverJackpotChance = new Stat(() => SetChanceTo);
        GameStore.CloverJackpotHoneyGain = new Stat(() => SetHoneyGainTo);
    }
}
