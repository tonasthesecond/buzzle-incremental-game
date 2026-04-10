using Godot;

[GlobalClass]
public partial class YarrowHoneyGainUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    [Export]
    public int BaseCost { get; set; } = 10;

    public override string GetText() =>
        $"{Style.CK("Yarrow Honey Yield", "noun_yarrow")} {Style.NumberChange(GameStore.YarrowHoneyGain.Value, GameStore.YarrowHoneyGain.Value + IncreaseBy)}";

    public override int GetCost() => Level * BaseCost;

    public override void Apply()
    {
        GameStore.YarrowHoneyGain.AddFlat(Name, IncreaseBy * Level);
    }
}
