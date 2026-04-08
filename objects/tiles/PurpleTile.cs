using Godot;

public partial class PurpleTile : BaseTile
{
    protected override void ModifyFlower(Flower flower)
    {
        flower.PollinationTime.AddPercent(
            "purple_tile",
            -GameStore.SmoothSoilPollinationTimeReductionBuff.Value
        );
    }
}
