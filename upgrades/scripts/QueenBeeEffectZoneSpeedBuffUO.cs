using Godot;

[GlobalClass]
public partial class QueenBeeEffectZoneSpeedBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.1f;

    [Export]
    public int BaseCost { get; set; } = 10;

    public override string GetText() =>
        $"{Style.CK("Queen Bee Speed Buff", "noun_queen_bee")} {Style.NumberChange(GameStore.QueenBeeEffectZoneSpeedBuff.Value, GameStore.QueenBeeEffectZoneSpeedBuff.Value + IncreaseBy)}x";

    public override int GetCost() => Level * BaseCost;

    public override void Apply()
    {
        GameStore.QueenBeeEffectZoneSpeedBuff.AddFlat(Name, IncreaseBy * Level);
    }
}
