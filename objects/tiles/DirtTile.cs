using Godot;

[GlobalClass]
public partial class DirtTile : BaseTile, IHasHoverTitle, IHasHoverDescription
{
    public override string GetHoverTitle() => "Dirt Tile";

    public override string GetHoverDescription()
    {
        string desc = "Boring dirt. You can put stuff on it?";
        if (GameStore.DirtPoppyHoneyGainBuff.Value > 0)
        {
            desc = "Basic earth.";
            desc +=
                $"\n\n{Style.CK("Poppies", "noun_poppy")} on this tile produce +{Style.CKPercent(GameStore.DirtPoppyHoneyGainBuff.Value)} honey";
        }
        return desc;
    }

    protected override void ModifyFlower(Flower flower)
    {
        if (flower is Poppy poppy)
            poppy.HoneyGain.AddPercent("dirt", GameStore.DirtPoppyHoneyGainBuff.Value);
    }
}
