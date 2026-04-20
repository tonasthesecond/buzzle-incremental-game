using Godot;

[GlobalClass]
public partial class QueenBeeEffectZoneRadiusUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 16f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Queen bees", "noun_queen")} influence {Style.CK("bees", "noun_bee")} upto {Style.NC(Utils.PixelsToTiles(GameStore.QueenBeeEffectZoneRadius.Value), Utils.PixelsToTiles(GameStore.QueenBeeEffectZoneRadius.Value + IncreaseBy))} away"
        + " tiles";

    public override void Apply() =>
        GameStore.QueenBeeEffectZoneRadius.AddFlat(Name, IncreaseBy * Level);
}
