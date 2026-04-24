using Godot;

[GlobalClass]
public partial class QueenBeeEffectZoneRadiusSpeedBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseRadiusBy { get; set; } = 16f;

    [Export]
    public float IncreaseSpeedBuffBy { get; set; } = 0.1f;

    public override string GetTechnicalText()
    {
        float radius = GameStore.QueenBeeEffectZoneRadius.Value;
        float speed = GameStore.QueenBeeEffectZoneSpeedBuff.Value;
        bool show = !IsMaxLevel();

        return $"{Style.CK("Queen bee", "noun_queen")} aura range {Style.NC(Utils.PixelsToTiles(radius), Utils.PixelsToTiles(radius + IncreaseRadiusBy), 0, show)} tiles\n"
            + $"{Style.CK("Queen bee", "noun_queen")} speed buff {Style.NCPercent(speed, speed + IncreaseSpeedBuffBy, show)}";
    }

    public override void Apply()
    {
        GameStore.QueenBeeEffectZoneRadius.AddFlat(Name, IncreaseRadiusBy * Level);
        GameStore.QueenBeeEffectZoneSpeedBuff.AddFlat(Name, IncreaseSpeedBuffBy * Level);
    }
}
