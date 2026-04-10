using Godot;

[GlobalClass]
public partial class CloverJackpotHoneyGainUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 2f;

    [Export]
    public int BaseCost { get; set; } = 10;

    public override string GetText() =>
        $"{Style.CK("Clover Jackpot Yield", "noun_clover")} {Style.NumberChange(GameStore.CloverJackpotHoneyGain.Value, GameStore.CloverJackpotHoneyGain.Value + IncreaseBy)}";

    public override int GetCost() => Level * BaseCost;

    public override void Apply()
    {
        GameStore.CloverJackpotHoneyGain.AddFlat(Name, IncreaseBy * Level);
    }
}
