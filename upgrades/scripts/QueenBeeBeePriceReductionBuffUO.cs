using Godot;

[GlobalClass]
public partial class QueenBeeBeePriceReductionBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.1f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Queen Bees", "noun_queen")} reduce {Style.CK("bees", "noun_bee")}' price by {Style.NumberChangePercent(GameStore.QueenBeeBeePriceReductionBuff.Value, GameStore.QueenBeeBeePriceReductionBuff.Value + IncreaseBy)}";

    public override void Apply() =>
        GameStore.QueenBeeBeePriceReductionBuff.AddFlat(Name, IncreaseBy * Level);
}
