using System;
using Godot;

public partial class BeekeeperFadeoutTimeUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    private float value => GameStore.BeekeeperEffectZoneFadeoutTime.Value * 2;

    public override string GetText() =>
        $"{Style.CK("Beekeeper Fadeout Time", "noun_beekeeper")} decreases by {Style.NumberChange(Utils.PixelsToTiles(value), Utils.PixelsToTiles(value + IncreaseBy))} tiles.";

    public override void Apply()
    {
        GameStore.BeekeeperEffectZoneFadeoutTime.AddFlat(Name, IncreaseBy * Level);
    }
}
