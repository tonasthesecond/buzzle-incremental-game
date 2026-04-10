using Godot;

[GlobalClass]
public partial class SunflowerHoneyCostUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    [Export]
    public int BaseCost { get; set; } = 10;

    public override string GetText() =>
        $"{Style.CK("Sunflower Honey Cost", "noun_sunflower")} {Style.NumberChange(GameStore.SunflowerHoneyCost.Value, GameStore.SunflowerHoneyCost.Value + IncreaseBy)}";

    public override int GetCost() => Level * BaseCost;

    public override void Apply()
    {
        GameStore.SunflowerHoneyCost.AddFlat(Name, IncreaseBy * Level);
    }
}
