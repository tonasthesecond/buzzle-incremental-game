using Godot;

[GlobalClass]
public partial class RosePerEmptyNeighborHoneyGainBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.05f;

    public override string GetHoverDescription() => 
        $"{Style.CK("Rose Empty Neighbor Bonus", "noun_rose")} {Style.NumberChange(GameStore.RosePerEmptyNeighborHoneyGainBuff.Value * 100f, (GameStore.RosePerEmptyNeighborHoneyGainBuff.Value + IncreaseBy) * 100f)}"
        + "% per empty";

    public override void Apply()
    {
        GameStore.RosePerEmptyNeighborHoneyGainBuff.AddFlat(Name, IncreaseBy * Level);
    }
}
