using Godot;

[GlobalClass]
public partial class QueenBeeEffectZoneSpeedBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.1f;

    public override string GetHoverDescription() => 
        $"{Style.CK("Queen Bee Speed Buff", "noun_queen_bee")} {Style.NumberChange(GameStore.QueenBeeEffectZoneSpeedBuff.Value, GameStore.QueenBeeEffectZoneSpeedBuff.Value + IncreaseBy)}x";

    public override void Apply()
    {
        GameStore.QueenBeeEffectZoneSpeedBuff.AddFlat(Name, IncreaseBy * Level);
    }
}
