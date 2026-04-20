using Godot;

[GlobalClass]
public partial class Rose : Flower
{
    public override Stat HoneyCost { set; get; } = new(() => GameStore.RoseHoneyCost.Value);
    public override Stat PollinationTime { set; get; } =
        new(() => GameStore.RosePollinationTime.Value);
    public override Stat HoneyGain { set; get; }

    public Rose()
    {
        HoneyGain = new(() =>
        {
            // flat bonus per manhattan distance from nearest hive
            int dist = TilesFromHive();
            float distBonus = dist * GameStore.RosePerTileFromHiveHoneyGainBonus.Value;

            // percent buff per empty neighbor (tile exists, no object)
            int emptyNeighbors = 8 - GetNeighborCount();
            float buff = 1f + emptyNeighbors * GameStore.RosePerEmptyNeighborHoneyGainBuff.Value;

            return (GameStore.RoseHoneyGain.Value + distBonus) * buff;
        });
    }

    public override string GetHoverDescription()
    {
        int dist = TilesFromHive();

        int emptyNeighbors = 8 - GetNeighborCount();

        return $"Rose | dist: {dist} | empty neighbors: {emptyNeighbors} | honey: {HoneyGain.Value:F2}";
    }

    private int TilesFromHive()
    {
        var grid = Services.Get<Grid>();
        var hive = grid.GetClosestObjectOfType<Hive>(GlobalPosition);
        if (hive == null)
            return 0;
        return Mathf.Abs(GridPosition.X - hive.GridPosition.X)
            + Mathf.Abs(GridPosition.Y - hive.GridPosition.Y);
    }

    private int GetNeighborCount()
    {
        var grid = Services.Get<Grid>();
        int count = 0;
        for (int dx = -1; dx <= 1; dx++)
        for (int dy = -1; dy <= 1; dy++)
        {
            if (dx == 0 && dy == 0)
                continue;
            var neighbor = GridPosition + new Vector2I(dx, dy);
            if (grid.HasTile(neighbor) && grid.HasObject(neighbor))
                count++;
        }
        return count;
    }

    protected override void OnPollinated()
    {
        base.OnPollinated();
        if (GetNeighborCount() == 0)
            sprite.Play("isolated");
    }
}
