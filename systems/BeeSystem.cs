using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class BeeSystem : GameSystem
{
    [Signal]
    public delegate void OnBeeSpawnedEventHandler(BeeEntity bee);

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
        var withHoney = grid.GetObjectsOfType<BaseFlower>().Where(f => f.Honey > 0).ToArray();
        var withoutHoney = grid.GetObjectsOfType<BaseFlower>().Where(f => f.Honey <= 0).ToArray();

        foreach (var bee in GetIdleBees())
        {
            if (!bee.Definition.ReceivesWorkJobs)
                continue;
            var job = bee.Definition.SelectJob(bee, withHoney, withoutHoney);
            if (job != null)
                bee.SetJob(job);
        }
    }

    /// Spawn a bee by scene name (used by save load).
    public BeeEntity? SpawnBee(string sceneName, HiveGridObject home)
    {
        var scene = GD.Load<PackedScene>($"res://objects/bees/{sceneName}.tscn");
        if (scene == null)
        {
            GD.PushError($"[BeeSystem] Missing bee scene: {sceneName}");
            return null;
        }
        return SpawnBee(scene, home);
    }

    /// Spawn a bee by scene (used by placement system).
    public BeeEntity? SpawnBee(PackedScene scene, HiveGridObject home)
    {
        if (home.BeeCount >= GameStore.HiveCapacityBee.Value)
        {
            GD.Print($"[BeeSystem] Hive at {home.GridPosition} is full ({home.BeeCount} bees)");
            return null;
        }
        var bee = scene.Instantiate<BeeEntity>();
        GetParent().AddChild(bee);
        bee.Setup(home);
        EmitSignal(SignalName.OnBeeSpawned, bee);
        return bee;
    }

    public BeeEntity? SpawnBeeAnywhere(PackedScene scene)
    {
        var hives = Services.Get<Grid>().GetObjectsOfType<HiveGridObject>();
        if (hives.Length == 0)
            return null;
        return SpawnBee(scene, Utils.GetRandom(hives));
    }

    public BeeEntity[] GetBees()
    {
        var bees = new List<BeeEntity>();
        foreach (Node node in GetTree().GetNodesInGroup("bees"))
            if (node is BeeEntity bee)
                bees.Add(bee);
        return bees.ToArray();
    }

    public int GetBeeCount() => GetBees().Length;

    public BeeEntity[] GetIdleBees() => GetBees().Where(b => b.job is IdleJob).ToArray();

    public BeeEntity[] GetBeesWithJob<T>()
        where T : IBeeJob => GetBees().Where(b => b.job is T).ToArray();
}
