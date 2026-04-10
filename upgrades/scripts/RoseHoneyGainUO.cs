using Godot;

[GlobalClass]
public partial class RoseHoneyGainUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    [Export]
    public int BaseCost { get; set; } = 10;

    public override string GetText() =>
        $"{Style.CK("Rose Honey Yield", "noun_rose")} {Style.NumberChange(GameStore.RoseHoneyGain.Value, GameStore.RoseHoneyGain.Value + IncreaseBy)}";

    public override int GetCost() => Level * BaseCost;

    public override void Apply()
    {
        GameStore.RoseHoneyGain.AddFlat(Name, IncreaseBy * Level);
    }
}
