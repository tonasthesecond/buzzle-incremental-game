using Godot;

[GlobalClass]
public partial class QueenBeeEffectZoneRadiusSpeedBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseRadiusBy { get; set; } = 16f;

    [Export]
    public float IncreaseSpeedBuffBy { get; set; } = 0.1f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Queen bees", "noun_queen")} speed up bee upto {Style.NumberChange(Utils.PixelsToTiles(GameStore.QueenBeeEffectZoneRadius.Value), Utils.PixelsToTiles(GameStore.QueenBeeEffectZoneRadius.Value + IncreaseRadiusBy))} tiles away\n{Style.CK("Queen bees", "noun_queen")} increase affected {Style.CK("bees", "noun_bee")}' speed by {Style.NumberChangePercent(GameStore.QueenBeeEffectZoneSpeedBuff.Value, GameStore.QueenBeeEffectZoneSpeedBuff.Value + IncreaseSpeedBuffBy)}";

    public override void Apply()
    {
        GameStore.QueenBeeEffectZoneRadius.AddFlat(Name, IncreaseRadiusBy * Level);
        GameStore.QueenBeeEffectZoneSpeedBuff.AddFlat(Name, IncreaseSpeedBuffBy * Level);
    }
}
