using Godot;

[GlobalClass]
public partial class CloverJackpotChanceJackpotHoneyGainSetUO : IUpgradeOption
{
    [Export]
    public float SetChanceTo { get; set; } = 0.05f;

    [Export]
    public float SetHoneyGainTo { get; set; } = 2f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Clover", "noun_clover")} jackpot chance {Style.NCPercent(GameStore.CloverJackpotChance.Value, SetChanceTo, !IsMaxLevel())}\n"
        + $"{Style.CK("Clover", "noun_clover")} jackpot yield {Style.NC(GameStore.CloverJackpotHoneyGain.Value, SetHoneyGainTo, showChange: !IsMaxLevel())} honey";

    public override void Apply()
    {
        if (Level <= 0)
            return;
        GameStore.CloverJackpotChance.RemoveAll();
        GameStore.CloverJackpotHoneyGain.RemoveAll();
        GameStore.CloverJackpotChance = new Stat(() => SetChanceTo);
        GameStore.CloverJackpotHoneyGain = new Stat(() => SetHoneyGainTo);
    }
}
