using Godot;

[GlobalClass]
public partial class LoamPollinationTimeReductionBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.05f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Loam", "noun_loam")} tiles reduce pollination time by {Style.NCPercent(GameStore.LoamPollinationTimeReductionBuff.Value, GameStore.LoamPollinationTimeReductionBuff.Value + IncreaseBy, !IsMaxLevel())}";

    public override void Apply() => GameStore.LoamPollinationTimeReductionBuff.AddFlat(Name, IncreaseBy * Level);
}
