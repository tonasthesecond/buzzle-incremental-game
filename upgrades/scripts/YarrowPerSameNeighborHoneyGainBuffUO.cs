using Godot;

[GlobalClass]
public partial class YarrowPerSameNeighborHoneyGainBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.05f;

    public override string GetHoverDescription() => 
        $"{Style.CK("Yarrow Neighbor Bonus", "noun_yarrow")} {Style.NumberChange(GameStore.YarrowPerSameNeighborHoneyGainBuff.Value * 100f, (GameStore.YarrowPerSameNeighborHoneyGainBuff.Value + IncreaseBy) * 100f)}"
        + "% per same neighbor";

    public override void Apply()
    {
        GameStore.YarrowPerSameNeighborHoneyGainBuff.AddFlat(Name, IncreaseBy * Level);
    }
}
