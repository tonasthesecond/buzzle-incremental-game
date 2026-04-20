using Godot;

[GlobalClass]
public partial class QueenBeeEffectZoneEverythingUO : IUpgradeOption
{
    [Export]
    public float IncreaseBeeSpeedBuffBy { get; set; } = 0.1f;

    [Export]
    public float IncreaseEffectZoneRadiusBy { get; set; } = 8f;

    [Export]
    public float IncreaseBeePollinationTimeReductionBuffBy { get; set; } = 0.1f;

    public override string GetTechnicalText()
    {
        string desc = "\n";

        desc +=
            $"\n{Style.CK("Queen Bee", "noun_queen")} boost {Style.CK("bees", "noun_bee")}' speed by {Style.NC(GameStore.QueenBeeEffectZoneSpeedBuff.Value, GameStore.QueenBeeEffectZoneSpeedBuff.Value + IncreaseBeeSpeedBuffBy)}";

        desc +=
            $"\n{Style.CK("Queen Bee", "noun_queen")} boost {Style.CK("bees", "noun_bee")}' upto {Style.NC(Utils.PixelsToTiles(GameStore.QueenBeeEffectZoneRadius.Value), Utils.PixelsToTiles(GameStore.QueenBeeEffectZoneRadius.Value + IncreaseEffectZoneRadiusBy))}";

        desc +=
            $"\n{Style.CK("Queen Bee", "noun_queen")} boost {Style.CK("bees", "noun_bee")}' pollination speed by {Style.NC(GameStore.QueenBeeEffectZonePollinationTimeReductionBuff.Value, GameStore.QueenBeeEffectZonePollinationTimeReductionBuff.Value + IncreaseBeePollinationTimeReductionBuffBy)}";

        return desc;
    }

    public override void Apply()
    {
        GameStore.QueenBeeEffectZoneRadius.AddFlat(Name, IncreaseEffectZoneRadiusBy * Level);
        GameStore.QueenBeeEffectZoneSpeedBuff.AddFlat(Name, IncreaseBeeSpeedBuffBy * Level);
        GameStore.QueenBeeEffectZonePollinationTimeReductionBuff.AddFlat(
            Name,
            IncreaseBeePollinationTimeReductionBuffBy * Level
        );
    }
}
