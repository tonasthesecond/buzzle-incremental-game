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
            var grid = Services.Get<Grid>();

            // flat bonus per manhattan distance from nearest hive
            var hive = grid.GetClosestObjectOfType<Hive>(GlobalPosition);
            float distBonus = 0f;
            if (hive != null)
            {
                var diff = GridPosition - hive.GridPosition;
                int dist = Mathf.Abs(diff.X) + Mathf.Abs(diff.Y);
                distBonus = dist * GameStore.RosePerTileFromHiveHoneyGainBonus.Value;
            }

            // percent buff per empty neighbor (tile exists, no object)
            int emptyNeighbors = 0;
            for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                    continue;
                var neighbor = GridPosition + new Vector2I(dx, dy);
                if (grid.HasTile(neighbor) && !grid.HasObject(neighbor))
                    emptyNeighbors++;
            }
            float buff = 1f + emptyNeighbors * GameStore.RosePerEmptyNeighborHoneyGainBuff.Value;

            return (GameStore.RoseHoneyGain.Value + distBonus) * buff;
        });
    }

    public override string GetHoverDescription()
    {
        var grid = Services.Get<Grid>();

        int dist = 0;
        var hive = grid.GetClosestObjectOfType<Hive>(GlobalPosition);
        if (hive != null)
        {
            var diff = GridPosition - hive.GridPosition;
            dist = Mathf.Abs(diff.X) + Mathf.Abs(diff.Y);
        }

        int emptyNeighbors = 0;
        for (int dx = -1; dx <= 1; dx++)
        for (int dy = -1; dy <= 1; dy++)
        {
            if (dx == 0 && dy == 0)
                continue;
            var neighbor = GridPosition + new Vector2I(dx, dy);
            if (grid.HasTile(neighbor) && !grid.HasObject(neighbor))
                emptyNeighbors++;
        }

        return $"Rose | dist: {dist} | empty neighbors: {emptyNeighbors} | honey: {HoneyGain.Value:F2}";
    }
}
