public partial class LoamTile : BaseTile
{
    protected override void ModifyFlower(Flower flower)
    {
        flower.PollinationTime.AddPercent(
            "purple_tile",
            -GameStore.LoamPollinationTimeReductionBuff.Value
        );
    }
}
