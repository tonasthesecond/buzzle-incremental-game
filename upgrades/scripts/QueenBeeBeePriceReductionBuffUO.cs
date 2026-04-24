using Godot;

[GlobalClass]
public partial class QueenBeeBeePriceReductionBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.1f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Queen bees", "noun_queen")} reduce all {Style.CK("bee", "noun_bee")} prices by {Style.NCPercent(GameStore.QueenBeeBeePriceReductionBuff.Value, GameStore.QueenBeeBeePriceReductionBuff.Value + IncreaseBy, !IsMaxLevel())}";

    public override void Apply() => GameStore.QueenBeeBeePriceReductionBuff.AddFlat(Name, IncreaseBy * Level);
}
