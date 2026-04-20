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

    public override string GetHoverDescription()
    {
        string desc = "A lush grassy tile.";
        desc +=
            $"\n\n{Style.CK("Flowers", "noun_flower")} produce +{Style.CKPercent(GameStore.GrassHoneyGainBuff.Value)} more honey on this tile";
        if (GameStore.GrassCloverJackpotChanceBonus.Value > 0)
            desc +=
                $"\n\n{Style.CK("Clovers", "noun_clover")} on this {Style.CK("tile", "noun_tile")} have +{Style.CK(GameStore.GrassCloverJackpotChanceBonus.Value.ToString("F0"))} of jackpot";
        return desc;
    }
}
