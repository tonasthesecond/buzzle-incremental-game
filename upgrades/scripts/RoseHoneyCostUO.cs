using Godot;

[GlobalClass]
public partial class RoseHoneyCostUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    [Export]
    public int BaseCost { get; set; } = 10;

    public override string GetText() =>
        $"{Style.CK("Rose Honey Cost", "noun_rose")} {Style.NumberChange(GameStore.RoseHoneyCost.Value, GameStore.RoseHoneyCost.Value + IncreaseBy)}";

    public override int GetCost() => Level * BaseCost;

    public override void Apply()
    {
        GameStore.RoseHoneyCost.AddFlat(Name, IncreaseBy * Level);
    }
}
