using Godot;

[GlobalClass]
public partial class CloverJackpotHoneyGainUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 2f;

    public override string GetTechnicalText() => 
        $"{Style.CK("Clover Jackpot Yield", "noun_clover")} {Style.NumberChange(GameStore.CloverJackpotHoneyGain.Value, GameStore.CloverJackpotHoneyGain.Value + IncreaseBy)}";

    public override void Apply()
    {
        GameStore.CloverJackpotHoneyGain.AddFlat(Name, IncreaseBy * Level);
    }
}
