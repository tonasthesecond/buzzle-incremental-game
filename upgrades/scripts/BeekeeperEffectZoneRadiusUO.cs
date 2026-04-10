using Godot;

[GlobalClass]
public partial class BeekeeperEffectZoneRadiusUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 16f;

    private float value => GameStore.BeekeeperEffectZoneRadius.Value * 2;

    public override string GetHoverDescription() => 
        $"{Style.CK("Beekeeper Effect Zone", "noun_beekeeper")} covers {Style.NumberChange(Utils.PixelsToTiles(value), Utils.PixelsToTiles(value + IncreaseBy))} tiles.";

    public override void Apply()
    {
        GameStore.BeekeeperEffectZoneRadius.AddFlat(Name, IncreaseBy * Level);
    }
}
