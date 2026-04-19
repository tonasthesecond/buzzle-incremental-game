using Godot;

[GlobalClass]
public partial class Yarrow : Flower
{
    public override Stat HoneyCost { set; get; } = new(() => GameStore.YarrowHoneyCost.Value);
    public override Stat PollinationTime { set; get; } =
        new(() => GameStore.YarrowPollinationTime.Value);
    public override Stat HoneyGain { set; get; }

    public Yarrow()
    {
        HoneyGain = new(() =>
        {
            int neighbors = GetYarrowNeighbors();
            return GameStore.YarrowHoneyGain.Value
                * (1f + neighbors * GameStore.YarrowPerSameNeighborHoneyGainBuff.Value);
        });
    }

    public override string GetHoverDescription()
    {
        int neighbors = GetYarrowNeighbors();
        return $"Yarrow | neighbors: {neighbors} | honey: {HoneyGain.Value:F2}";
    }

    private int GetYarrowNeighbors()
    {
        Grid grid = Services.Get<Grid>()!;
        int neighbors = 0;
        for (int dx = -1; dx <= 1; dx++)
        for (int dy = -1; dy <= 1; dy++)
        {
            if (dx == 0 && dy == 0)
                continue;
            if (grid.GetObjectAt(GridPosition + new Vector2I(dx, dy)) is Yarrow)
                neighbors++;
        }
        return neighbors;
    }

    protected override void OnPollinated()
    {
        base.OnPollinated();
        if (GetYarrowNeighbors() == 8)
        {
            sprite.Play("max");
            return;
        }
        if (GetYarrowNeighbors() > 1)
            sprite.Play("plenty");
    }
}
