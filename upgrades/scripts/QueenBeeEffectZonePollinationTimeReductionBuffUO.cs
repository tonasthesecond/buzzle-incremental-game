using Godot;

[GlobalClass]
public partial class QueenBeeEffectZonePollinationTimeReductionBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.1f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Queen bee", "noun_queen")} aura reduces pollination time by {Style.NCPercent(GameStore.QueenBeeEffectZonePollinationTimeReductionBuff.Value, GameStore.QueenBeeEffectZonePollinationTimeReductionBuff.Value + IncreaseBy, !IsMaxLevel())}";

    public override void Apply() => GameStore.QueenBeeEffectZonePollinationTimeReductionBuff.AddFlat(Name, IncreaseBy * Level);
}
