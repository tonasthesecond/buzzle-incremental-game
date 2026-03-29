using Godot;

[GlobalClass]
public partial class BeekeeperEffectZoneRadiusUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 16f;

    private float value => GameStore.BeekeeperRadius.Value * 2;

    public override string GetText() =>
        $"{Style.CK("Beekeeper Effect Zone", "noun_beekeeper")} covers {Style.NumberChange(Utils.PixelsToTiles(value), Utils.PixelsToTiles(value + IncreaseBy))} tiles.";

    public override void Apply()
    {
        GameStore.BeekeeperRadius.AddFlat(Name, IncreaseBy * Level);
    }
}
