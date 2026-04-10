public partial class GrassTile : BaseTile, IHasHoverTitle, IHasHoverDescription
{
    protected override void ModifyFlower(Flower flower)
    {
        flower.HoneyGain.AddPercent("rich_soil", GameStore.RichSoilHoneyGainBuff.Value);
    }

    public override string GetHoverTitle() => "Grass Tile";

    public override string GetHoverDescription() =>
        $"{Style.CK("Flowers", "noun_flower")} produce {Style.CKPercent(GameStore.RichSoilHoneyGainBuff.Value)} more honey on this tile.";
}
