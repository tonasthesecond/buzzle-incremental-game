using Godot;

[GlobalClass]
public partial class QueenBeeEffectZoneRadiusUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 16f;

    [Export]
    public int BaseCost { get; set; } = 10;

    public override string GetText() =>
        $"{Style.CK("Queen Bee Zone", "noun_queen_bee")} {Style.NumberChange(Utils.PixelsToTiles(GameStore.QueenBeeEffectZoneRadius.Value * 2), Utils.PixelsToTiles(GameStore.QueenBeeEffectZoneRadius.Value * 2 + IncreaseBy))}"
        + " tiles";

    public override int GetCost() => Level * BaseCost;

    public override void Apply()
    {
        GameStore.QueenBeeEffectZoneRadius.AddFlat(Name, IncreaseBy * Level);
    }
}
