using Godot;

[GlobalClass]
public partial class CloverJackpotChanceJackpotHoneyGainUO : IUpgradeOption
{
    [Export]
    public float IncreaseChanceBy { get; set; } = 0.05f;

    [Export]
    public float IncreaseHoneyGainBy { get; set; } = 2f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Clovers", "noun_clover")} have a {Style.NCPercent(GameStore.CloverJackpotChance.Value, GameStore.CloverJackpotChance.Value + IncreaseChanceBy)} of jackpot\n{Style.CK("Clovers", "noun_clover")} produce {Style.NC(GameStore.CloverJackpotHoneyGain.Value, GameStore.CloverJackpotHoneyGain.Value + IncreaseHoneyGainBy)} honey on jackpot";

    public override void Apply()
    {
        GameStore.CloverJackpotChance.AddFlat(Name, IncreaseChanceBy * Level);
        GameStore.CloverJackpotHoneyGain.AddFlat(Name, IncreaseHoneyGainBy * Level);
    }
}
