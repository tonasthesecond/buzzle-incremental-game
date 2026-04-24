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
        float radius = GameStore.QueenBeeEffectZoneRadius.Value;
        float speed = GameStore.QueenBeeEffectZoneSpeedBuff.Value;
        float pollination = GameStore.QueenBeeEffectZonePollinationTimeReductionBuff.Value;
        bool show = !IsMaxLevel();

        return $"{Style.CK("Queen bee", "noun_queen")} aura range {Style.NC(Utils.PixelsToTiles(radius), Utils.PixelsToTiles(radius + IncreaseEffectZoneRadiusBy), 0, show)} tiles\n"
            + $"{Style.CK("Queen bee", "noun_queen")} speed buff {Style.NCPercent(speed, speed + IncreaseBeeSpeedBuffBy, show)}\n"
            + $"{Style.CK("Queen bee", "noun_queen")} pollination speed buff {Style.NCPercent(pollination, pollination + IncreaseBeePollinationTimeReductionBuffBy, show)}";
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
