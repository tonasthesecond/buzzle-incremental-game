using Godot;

[GlobalClass]
public partial class GrassCloverJackpotChanceBonusUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.05f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Grass", "noun_grass")} tiles boost {Style.CK("Clovers'", "noun_clover")} jackpot chance by {Style.NCPercent(GameStore.GrassCloverJackpotChanceBonus.Value, GameStore.GrassCloverJackpotChanceBonus.Value + IncreaseBy, !IsMaxLevel())}";

    public override void Apply() => GameStore.GrassCloverJackpotChanceBonus.AddFlat(Name, IncreaseBy * Level);
}
