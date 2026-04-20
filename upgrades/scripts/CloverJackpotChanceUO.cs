using Godot;

[GlobalClass]
public partial class CloverJackpotChanceUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.05f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Clovers", "noun_clover")} have a {Style.NCPercent(GameStore.CloverJackpotChance.Value, GameStore.CloverJackpotChance.Value + IncreaseBy * Level)} chance to jackpot";

    public override void Apply()
    {
        GameStore.CloverJackpotChance.AddFlat(Name, IncreaseBy * Level);
    }
}
