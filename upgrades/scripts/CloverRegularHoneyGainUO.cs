using Godot;

[GlobalClass]
public partial class CloverRegularHoneyGainUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    [Export]
    public int BaseCost { get; set; } = 10;

    public override string GetText() =>
        $"{Style.CK("Clover Base Yield", "noun_clover")} {Style.NumberChange(GameStore.CloverRegularHoneyGain.Value, GameStore.CloverRegularHoneyGain.Value + IncreaseBy)}";

    public override int GetCost() => Level * BaseCost;

    public override void Apply()
    {
        GameStore.CloverRegularHoneyGain.AddFlat(Name, IncreaseBy * Level);
    }
}
