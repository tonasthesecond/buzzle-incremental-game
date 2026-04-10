using Godot;

[GlobalClass]
public partial class SunflowerHoneyGainUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    [Export]
    public int BaseCost { get; set; } = 10;

    public override string GetText() =>
        $"{Style.CK("Sunflower Honey Yield", "noun_sunflower")} {Style.NumberChange(GameStore.SunflowerHoneyGain.Value, GameStore.SunflowerHoneyGain.Value + IncreaseBy)}";

    public override int GetCost() => Level * BaseCost;

    public override void Apply()
    {
        GameStore.SunflowerHoneyGain.AddFlat(Name, IncreaseBy * Level);
    }
}
