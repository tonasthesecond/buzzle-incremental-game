using Godot;

[GlobalClass]
public partial class Yarrow : Flower
{
    public override Stat HoneyCost { set; get; } = new(() => GameStore.YarrowHoneyCost.Value);
    public override Stat PollinationTime { set; get; } =
        new(() => GameStore.YarrowPollinationTime.Value);
    public override Stat HoneyGain { set; get; }

    const string YarrowNeighborsKey = "neighbor";

    public Yarrow()
    {
        HoneyGain = new(() => GameStore.YarrowHoneyGain.Value);

        // TEST: optimize by adding a condition
        SignalBus.Instance.GridObjectPlaced += (_) => UpdateNeighborBonus();
        SignalBus.Instance.GridObjectRemoved += (_) => UpdateNeighborBonus();

        // also update when the store value changes
        GameStore.YarrowPerSameNeighborHoneyGainBuff.Changed += UpdateNeighborBonus;
    }

    private void UpdateNeighborBonus()
    {
        if (!Placed)
            return;
        int neighbors = GetYarrowNeighbors();
        HoneyGain.AddFlat(
            YarrowNeighborsKey,
            neighbors * GameStore.YarrowPerSameNeighborHoneyGainBuff.Value
        );
    }

    protected override string GetTechnicalText()
    {
        string desc = base.GetTechnicalText();
        float bonusPerNeighbor = GameStore.YarrowPerSameNeighborHoneyGainBuff.Value;

        if (Placed)
        {
            int neighbors = GetYarrowNeighbors();
            desc +=
                $"\n{Style.CK("Yarrow Neighbor Buff", "noun_yarrow")}: +{Style.CKPercent((float)HoneyGain.Get(YarrowNeighborsKey))} ({neighbors} {Style.CK("Yarrow", "noun_yarrow")} neighbors)";
        }
        else
        {
            desc +=
                $"\n{Style.CK("Companionship", "noun_yarrow")}: +{Style.CKPercent(GameStore.YarrowPerSameNeighborHoneyGainBuff.Value)} per {Style.CK("Yarrow", "noun_yarrow")} neighbor";
        }
        return desc;
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
