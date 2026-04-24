using Godot;

[GlobalClass]
public partial class QueenBeeEffectZoneSpeedBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.1f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Queen bee", "noun_queen")} aura speeds up {Style.CK("bees", "noun_bee")} by {Style.NCPercent(GameStore.QueenBeeEffectZoneSpeedBuff.Value, GameStore.QueenBeeEffectZoneSpeedBuff.Value + IncreaseBy, !IsMaxLevel())}";

    public override void Apply() => GameStore.QueenBeeEffectZoneSpeedBuff.AddFlat(Name, IncreaseBy * Level);
}
