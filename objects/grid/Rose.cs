using Godot;

[GlobalClass]
public partial class Rose : Flower
{
    public override Stat HoneyCost { set; get; } = new(() => GameStore.RoseHoneyCost.Value);
    public override Stat PollinationTime { set; get; } =
        new(() => GameStore.RosePollinationTime.Value);
    public override Stat HoneyGain { set; get; } = new(() => GameStore.RoseHoneyGain.Value);

    const string RoseNeighborsKey = "neighbor";
    const string RoseIsolatedKey = "isolated";

    public Rose()
    {
        SignalBus.Instance.GridObjectPlaced += (_) => UpdateRoseBonuses();
        SignalBus.Instance.GridObjectRemoved += (_) => UpdateRoseBonuses();
        GameStore.RosePerTileFromHiveHoneyGainBonus.Changed += UpdateRoseBonuses;
        GameStore.RosePerEmptyNeighborHoneyGainBuff.Changed += UpdateRoseBonuses;
    }

    private void UpdateRoseBonuses()
    {
        if (!Placed)
            return;
        int dist = TilesFromHive();
        HoneyGain.AddFlat(
            RoseIsolatedKey,
            dist * GameStore.RosePerTileFromHiveHoneyGainBonus.Value
        );
        HoneyGain.AddPercent(
            RoseNeighborsKey,
            GetEmptyNeighbors() * GameStore.RosePerEmptyNeighborHoneyGainBuff.Value
        );
    }

    protected override string GetTechnicalText()
    {
        string desc = base.GetTechnicalText();

        if (Placed)
        {
            desc +=
                $"{Style.CK("Dist. From Hive Bonus")}: +{Style.CK(GameStore.RosePerTileFromHiveHoneyGainBonus.Value.ToString("F0"))} ({Style.CK(TilesFromHive().ToString())} honey {Style.CK("tiles", "noun_tile")})\n";
            desc +=
                $"{Style.CK("Empty Neighbors Buff")}: +{Style.CKPercent(GameStore.RosePerEmptyNeighborHoneyGainBuff.Value)} honey ({Style.CK(GetEmptyNeighbors().ToString())} empty neighbors)\n";
        }
        else
        {
            desc +=
                $"{Style.CK("Isolation Bonus")}: +{Style.CK(GameStore.RosePerTileFromHiveHoneyGainBonus.Value.ToString("F0"))} honey per tile from hive\n";
            desc +=
                $"{Style.CK("Antisocial Buff")}: +{Style.CKPercent(GameStore.RosePerEmptyNeighborHoneyGainBuff.Value)} honey per empty neighbor";
        }

        return desc;
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

    private int GetEmptyNeighbors() => 8 - GetNeighborCount();

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
