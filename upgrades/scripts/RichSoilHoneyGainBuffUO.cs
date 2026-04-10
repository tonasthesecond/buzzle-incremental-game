using Godot;

[GlobalClass]
public partial class RichSoilHoneyGainBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.05f;

    [Export]
    public int BaseCost { get; set; } = 10;

    public override string GetText() =>
        $"{Style.CK("Rich Soil Honey Buff", "noun_rich_soil")} {Style.NumberChange(GameStore.RichSoilHoneyGainBuff.Value * 100f, (GameStore.RichSoilHoneyGainBuff.Value + IncreaseBy) * 100f)}"
        + "%";

    public override int GetCost() => Level * BaseCost;

    public override void Apply()
    {
        GameStore.RichSoilHoneyGainBuff.AddFlat(Name, IncreaseBy * Level);
    }
}
