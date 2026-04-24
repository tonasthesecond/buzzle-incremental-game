using Godot;

[GlobalClass]
public partial class QueenBeeEffectZoneRadiusUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 16f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Queen bee", "noun_queen")} aura reaches {Style.NC(Utils.PixelsToTiles(GameStore.QueenBeeEffectZoneRadius.Value), Utils.PixelsToTiles(GameStore.QueenBeeEffectZoneRadius.Value + IncreaseBy), showChange: !IsMaxLevel())} tiles";

    public override void Apply() => GameStore.QueenBeeEffectZoneRadius.AddFlat(Name, IncreaseBy * Level);
}
