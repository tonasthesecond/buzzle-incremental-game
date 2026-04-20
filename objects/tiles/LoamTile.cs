public partial class LoamTile : BaseTile, IHasHoverTitle, IHasHoverDescription, IHasHoverPrice
{
    protected override void ModifyFlower(Flower flower)
    {
        flower.PollinationTime.AddPercent(
            "loam_tile",
            -GameStore.LoamPollinationTimeReductionBuff.Value
        );
        if (flower is Yarrow yarrow)
            yarrow.HoneyGain.AddPercent("loam", GameStore.LoamYarrowHoneyGainBuff.Value);
    }

    public override string GetHoverTitle() => "Loam Tile";

    public override string GetHoverDescription()
    {
        string desc = "Soft soil fit for root growth.";

        desc +=
            $"\n\n{Style.CK("Flowers", "noun_flower")} pollinate +{Style.CKPercent(GameStore.LoamPollinationTimeReductionBuff.Value)} faster on this tile.";

        if (GameStore.LoamYarrowHoneyGainBuff.Value > 0)
            desc +=
                $"\n\n{Style.CK("Yarrows", "noun_yarrow")} on this tile produce +{Style.CKPercent(GameStore.LoamYarrowHoneyGainBuff.Value)} honey";

        return desc;
    }
}
