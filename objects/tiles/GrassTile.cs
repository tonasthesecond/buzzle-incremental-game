public partial class GrassTile : BaseTile, IHasHoverTitle, IHasHoverDescription
{
    protected override void ModifyFlower(Flower flower)
    {
        flower.HoneyGain.AddPercent("grass", GameStore.GrassHoneyGainBuff.Value);
        if (flower is Clover clover)
            GameStore.CloverJackpotChance.AddPercent(
                "natural habitat",
                GameStore.GrassCloverJackpotChanceBonus.Value
            );
    }

    public override string GetHoverTitle() => "Grass Tile";

    public override string GetHoverDescription() =>
        $"{Style.CK("Flowers", "noun_flower")} produce {Style.CKPercent(GameStore.GrassHoneyGainBuff.Value)} more honey on this tile.";
}
