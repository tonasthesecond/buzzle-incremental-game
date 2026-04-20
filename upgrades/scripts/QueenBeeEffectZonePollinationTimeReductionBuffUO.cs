using Godot;

[GlobalClass]
public partial class QueenBeeEffectZonePollinationTimeReductionBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.1f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Queen Bee", "noun_queen")} boost {Style.CK("bees", "noun_bee")}' pollination speed by {Style.NumberChange(GameStore.QueenBeeEffectZonePollinationTimeReductionBuff.Value, GameStore.QueenBeeEffectZonePollinationTimeReductionBuff.Value + IncreaseBy)}";

    public override void Apply()
    {
        GameStore.QueenBeeEffectZonePollinationTimeReductionBuff.AddFlat(Name, IncreaseBy * Level);
    }
}
