using Godot;

[GlobalClass]
public partial class QueenBeeEffectZoneSpeedBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.1f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Queens", "noun_queen")} {Style.NumberChange(GameStore.QueenBeeEffectZoneSpeedBuff.Value, GameStore.QueenBeeEffectZoneSpeedBuff.Value + IncreaseBy)}x";

    public override void Apply()
    {
        GameStore.QueenBeeEffectZoneSpeedBuff.AddFlat(Name, IncreaseBy * Level);
    }
}
