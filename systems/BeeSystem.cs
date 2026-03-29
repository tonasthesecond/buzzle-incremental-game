using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class BeeSystem : GameSystem
{
    [Signal]
    public delegate void OnBeeSpawnedEventHandler(Bee bee);

    private HashSet<BaseGridObject> claimedObjects = new();

    public bool IsClaimed(BaseGridObject obj) => claimedObjects.Contains(obj);

    public bool ClaimObject(BaseGridObject obj) => claimedObjects.Add(obj);

    public void ReleaseObject(BaseGridObject obj) => claimedObjects.Remove(obj);

    public void SpawnFromSave()
    {
        var grid = Services.Get<Grid>();
        foreach (var saved in GameStore.Save.Hives)
        {
            var hive = grid.GetObjectAt(new Vector2I(saved.X, saved.Y)) as HiveGridObject;
            if (hive == null)
                continue;
            foreach (var kv in saved.BeeCounts)
                for (int i = 0; i < kv.Value; i++)
                    SpawnBee(kv.Key, hive);
        }
    }

    public override void _Process(double delta)
    {
        var grid = Services.Get<Grid>();
        var flowersWithHoney = grid.GetObjectsOfType<BaseFlower>()
            .Where(f => f.Honey > 0)
            .ToArray();
        var flowersWithoutHoney = grid.GetObjectsOfType<BaseFlower>()
            .Where(f => f.Honey <= 0)
            .ToArray();

        foreach (var bee in GetIdleBees())
        {
            // queens manage their own behavior
            if (bee is QueenBee)
                continue;

            int harvesters = GetBeesWithJob<HarvesterJob>().Length;
            int pollinators = GetBeesWithJob<PollinatorJob>().Length;
            if (harvesters < flowersWithHoney.Length)
                bee.SetJob(new HarvesterJob());
            else if (pollinators < flowersWithoutHoney.Length)
                bee.SetJob(new PollinatorJob());
        }
    }

    /// Spawn a bee by type name string (used by save load).
    public Bee? SpawnBee(string typeName, HiveGridObject home)
    {
        var scene = GD.Load<PackedScene>($"res://objects/bees/{typeName}.tscn");
        if (scene == null)
        {
            GD.PushError($"[BeeSystem] Missing bee scene: {typeName}");
            return null;
        }
        return SpawnBee(scene, home);
    }

    /// Spawn a bee by scene (used by placement system).
    public Bee? SpawnBee(PackedScene scene, HiveGridObject home)
    {
        if (home.BeeCount >= GameStore.HiveCapacityBee.Value)
        {
            GD.Print($"[BeeSystem] Hive at {home.GridPosition} is full ({home.BeeCount} bees)");
            return null;
        }
        var bee = scene.Instantiate<Bee>();
        GetParent().AddChild(bee);
        bee.Setup(home, new IdleJob());
        EmitSignal(SignalName.OnBeeSpawned, bee);
        return bee;
    }

    /// Spawn a bee by type (used in code, e.g. BuildDefault).
    public Bee? SpawnBee<T>(HiveGridObject home)
        where T : Bee => SpawnBee(typeof(T).Name, home);

    public Bee? SpawnBeeAnywhere()
    {
        var hives = Services.Get<Grid>().GetObjectsOfType<HiveGridObject>();
        if (hives.Length == 0)
            return null;
        return SpawnBee<Bee>(Utils.GetRandom(hives));
    }

    public Bee[] GetBees()
    {
        var bees = new List<Bee>();
        foreach (Node node in GetTree().GetNodesInGroup("bees"))
            if (node is Bee bee)
                bees.Add(bee);
        return bees.ToArray();
    }

    public int GetBeeCount() => GetBees().Length;

    public Bee[] GetIdleBees() => GetBees().Where(b => b.job is IdleJob).ToArray();

    public Bee[] GetBeesWithJob<T>()
        where T : IBeeJob => GetBees().Where(b => b.job is T).ToArray();
}
