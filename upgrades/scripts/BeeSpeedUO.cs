using Godot;

[GlobalClass]
public partial class BeeSpeedUO : IUpgradeOption
{
    [Export]
    public int IncreaseBy { get; set; } = 10;

    private float value => GameStore.BeeSpeed.Value;

    public override string GetText() =>
        $"{Style.CK("Bees", "noun_bee")} moves {Style.NumberChange(Utils.PixelsToTiles(value), Utils.PixelsToTiles(value + IncreaseBy))} tiles per second.";

    public override int GetCost() => (Level + 1) * 10;

    public override void Apply()
    {
        GameStore.BeeSpeed.AddFlat(Name, IncreaseBy * Level);
    }
}
