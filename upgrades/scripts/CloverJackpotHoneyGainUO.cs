using Godot;

[GlobalClass]
public partial class CloverJackpotHoneyGainUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 2f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Clovers", "noun_clover")} yield {Style.NC(GameStore.CloverJackpotHoneyGain.Value, GameStore.CloverJackpotHoneyGain.Value + IncreaseBy, showChange: !IsMaxLevel())} honey on jackpot";

    public override void Apply() => GameStore.CloverJackpotHoneyGain.AddFlat(Name, IncreaseBy * Level);
}
