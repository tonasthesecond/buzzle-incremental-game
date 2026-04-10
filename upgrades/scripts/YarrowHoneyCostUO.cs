using Godot;

[GlobalClass]
public partial class YarrowHoneyCostUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    [Export]
    public int BaseCost { get; set; } = 10;

    public override string GetText() =>
        $"{Style.CK("Yarrow Honey Cost", "noun_yarrow")} {Style.NumberChange(GameStore.YarrowHoneyCost.Value, GameStore.YarrowHoneyCost.Value + IncreaseBy)}";

    public override int GetCost() => Level * BaseCost;

    public override void Apply()
    {
        GameStore.YarrowHoneyCost.AddFlat(Name, IncreaseBy * Level);
    }
}
