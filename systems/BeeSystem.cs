#nullable enable
using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class BeeSystem : GameSystem
{
    private PackedScene beeScene = GD.Load<PackedScene>("res://objects/Bee.tscn");
    private HashSet<BaseGridObject> claimedObjects = new();

    public bool IsClaimed(BaseGridObject obj) => claimedObjects.Contains(obj);

    public bool ClaimObject(BaseGridObject obj) => claimedObjects.Add(obj);

    public void ReleaseObject(BaseGridObject obj) => claimedObjects.Remove(obj);

    public override void _Ready()
    {
        // Callable.From(SpawnFromSave).CallDeferred();
    }

    public void SpawnFromSave()
    {
        Grid? grid = Services.Get<Grid>();
        foreach (SavedHive saved in GameStore.Save.Hives)
        {
            HiveGridObject? hive =
                grid.GetObjectAt(new Vector2I(saved.X, saved.Y)) as HiveGridObject;
            if (hive == null)
                continue;
            for (int i = 0; i < saved.BeeCount; i++)
            {
                SpawnBee(hive);
            }
        }
    }

    public override void _Process(double delta)
    {
        Grid? grid = Services.Get<Grid>();
        BaseFlower[] flowersWithHoney = grid.GetObjectsOfType<BaseFlower>()
            .Where(f => f.Honey > 0)
            .ToArray();
        BaseFlower[] flowersWithoutHoney = grid.GetObjectsOfType<BaseFlower>()
            .Where(f => f.Honey <= 0)
            .ToArray();

        foreach (Bee bee in GetIdleBees())
        {
            int harvesters = GetBeesWithJob<HarvesterJob>().Length;
            int pollinators = GetBeesWithJob<PollinatorJob>().Length;
            if (harvesters < flowersWithHoney.Length)
                bee.SetJob(new HarvesterJob());
            else if (pollinators < flowersWithoutHoney.Length)
                bee.SetJob(new PollinatorJob());
        }
    }

    public Bee? SpawnBee(HiveGridObject home)
    {
        if (home.BeeCount >= GameStore.HiveCapacityBee)
            return null;
        Bee bee = beeScene.Instantiate<Bee>();
        GetParent().AddChild(bee);
        bee.Setup(home, new IdleJob());
        return bee;
    }

    public Bee? SpawnBeeAnywhere()
    {
        HiveGridObject[] hives = Services.Get<Grid>().GetObjectsOfType<HiveGridObject>();
        if (hives.Length == 0)
            return null;
        return SpawnBee(Utils.GetRandom(hives));
    }

    public Bee[] GetBees()
    {
        List<Bee> bees = new List<Bee>();
        foreach (Node node in GetTree().GetNodesInGroup("bees"))
            if (node is Bee bee)
                bees.Add(bee);
        return bees.ToArray();
    }

    public Bee[] GetIdleBees() => GetBees().Where(b => b.job is IdleJob).ToArray();

    public Bee[] GetBeesWithJob<T>()
        where T : IBeeJob => GetBees().Where(b => b.job is T).ToArray();
}
