using Godot;

[GlobalClass]
public partial class PoppyHoneyGainUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    [Export]
    public int BaseCost { get; set; } = 10;

    public override string GetText() =>
        $"{Style.CK("Poppy Honey Yield", "noun_poppy")} {Style.NumberChange(GameStore.PoppyHoneyGain.Value, GameStore.PoppyHoneyGain.Value + IncreaseBy)}";

    public override int GetCost() => Level * BaseCost;

    public override void Apply()
    {
        GameStore.PoppyHoneyGain.AddFlat(Name, IncreaseBy * Level);
    }
}
