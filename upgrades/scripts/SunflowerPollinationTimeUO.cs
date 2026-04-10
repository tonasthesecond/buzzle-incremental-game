using Godot;

[GlobalClass]
public partial class SunflowerPollinationTimeUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    [Export]
    public int BaseCost { get; set; } = 10;

    public override string GetText() =>
        $"{Style.CK("Sunflower Pollination Time", "noun_sunflower")} {Style.NumberChange(GameStore.SunflowerPollinationTime.Value, GameStore.SunflowerPollinationTime.Value + IncreaseBy)}s";

    public override int GetCost() => Level * BaseCost;

    public override void Apply()
    {
        GameStore.SunflowerPollinationTime.AddFlat(Name, IncreaseBy * Level);
    }
}
