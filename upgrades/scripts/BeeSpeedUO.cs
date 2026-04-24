using Godot;

[GlobalClass]
public partial class BeeSpeedUO : IUpgradeOption
{
    [Export]
    public int IncreaseBy { get; set; } = 10;

    private float value => GameStore.BeeSpeed.Value;

    public override string GetTechnicalText() =>
        $"{Style.CK("Bees", "noun_bee")} move at {Style.NC(Utils.PixelsToTiles(value), Utils.PixelsToTiles(value + IncreaseBy), showChange: !IsMaxLevel())} tiles/s";

    public override void Apply() => GameStore.BeeSpeed.AddFlat(Name, IncreaseBy * Level);
}
