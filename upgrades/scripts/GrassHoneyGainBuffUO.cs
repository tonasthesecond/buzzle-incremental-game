using Godot;

[GlobalClass]
public partial class GrassHoneyGainBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.05f;

    public override string GetHoverDescription() =>
        $"{Style.CK("Rich Soil Honey Buff", "noun_rich_soil")} {Style.NumberChange(GameStore.GrassHoneyGainBuff.Value * 100f, (GameStore.GrassHoneyGainBuff.Value + IncreaseBy) * 100f)}"
        + "%";

    public override void Apply()
    {
        GameStore.GrassHoneyGainBuff.AddFlat(Name, IncreaseBy * Level);
    }
}
