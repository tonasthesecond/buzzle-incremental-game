using Godot;

[GlobalClass]
public partial class RichSoilHoneyGainBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.05f;

    public override string GetHoverDescription() => 
        $"{Style.CK("Rich Soil Honey Buff", "noun_rich_soil")} {Style.NumberChange(GameStore.RichSoilHoneyGainBuff.Value * 100f, (GameStore.RichSoilHoneyGainBuff.Value + IncreaseBy) * 100f)}"
        + "%";

    public override void Apply()
    {
        GameStore.RichSoilHoneyGainBuff.AddFlat(Name, IncreaseBy * Level);
    }
}
