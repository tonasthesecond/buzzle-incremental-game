public partial class LoamTile : BaseTile, IHasHoverTitle, IHasHoverDescription, IHasHoverPrice
{
    protected override void ModifyFlower(Flower flower)
    {
        flower.PollinationTime.AddPercent(
            "loam_tile",
            -GameStore.LoamPollinationTimeReductionBuff.Value
        );
    }

    public override string GetHoverTitle() => "Loam Tile";

    public override string GetHoverDescription() =>
        $"{Style.CK("Flowers", "noun_flower")} pollinate {Style.CKPercent(GameStore.LoamPollinationTimeReductionBuff.Value)} faster on this tile.";
}
