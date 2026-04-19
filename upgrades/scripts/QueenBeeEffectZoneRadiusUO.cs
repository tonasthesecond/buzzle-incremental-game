using Godot;

[GlobalClass]
public partial class QueenBeeEffectZoneRadiusUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 16f;

    public override string GetTechnicalText() => 
        $"{Style.CK("Queen Bee Zone", "noun_queen_bee")} {Style.NumberChange(Utils.PixelsToTiles(GameStore.QueenBeeEffectZoneRadius.Value * 2), Utils.PixelsToTiles(GameStore.QueenBeeEffectZoneRadius.Value * 2 + IncreaseBy))}"
        + " tiles";

    public override void Apply()
    {
        GameStore.QueenBeeEffectZoneRadius.AddFlat(Name, IncreaseBy * Level);
    }
}
