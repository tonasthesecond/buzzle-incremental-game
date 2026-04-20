using Godot;

[GlobalClass]
public partial class GrassCloverJackpotChanceBonusUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Grass", "noun_grass")} increases {Style.CK("Clovers", "noun_clover")}' {Style.CK("Jackpot Chance", "noun_jackpot")} by {Style.NumberChange(GameStore.CloverJackpotChance.Value, GameStore.CloverJackpotChance.Value + IncreaseBy)}";

    public override void Apply() =>
        GameStore.GrassCloverJackpotChanceBonus.AddFlat(Name, IncreaseBy * Level);
}
