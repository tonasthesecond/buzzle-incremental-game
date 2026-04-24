using Godot;

[GlobalClass]
public partial class CloverJackpotChanceJackpotHoneyGainUO : IUpgradeOption
{
    [Export]
    public float IncreaseChanceBy { get; set; } = 0.05f;

    [Export]
    public float IncreaseHoneyGainBy { get; set; } = 2f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Clover", "noun_clover")} jackpot chance {Style.NCPercent(GameStore.CloverJackpotChance.Value, GameStore.CloverJackpotChance.Value + IncreaseChanceBy, !IsMaxLevel())}\n"
        + $"{Style.CK("Clover", "noun_clover")} jackpot yield {Style.NC(GameStore.CloverJackpotHoneyGain.Value, GameStore.CloverJackpotHoneyGain.Value + IncreaseHoneyGainBy, showChange: !IsMaxLevel())} honey";

    public override void Apply()
    {
        GameStore.CloverJackpotChance.AddFlat(Name, IncreaseChanceBy * Level);
        GameStore.CloverJackpotHoneyGain.AddFlat(Name, IncreaseHoneyGainBy * Level);
    }
}
