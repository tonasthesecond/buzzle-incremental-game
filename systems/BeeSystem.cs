using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class BeeSystem : GameSystem
{
    [Signal]
    public delegate void BeeSpawnedEventHandler(Bee bee);

    [Signal]
    public delegate void BeeRemovedEventHandler();

    private Dictionary<BaseGridObject, Bee> claimedObjects = new();

    public bool IsClaimed(BaseGridObject obj) => claimedObjects.ContainsKey(obj);

    public bool ClaimObject(BaseGridObject obj, Bee bee) => claimedObjects.TryAdd(obj, bee);

    public void ReleaseObject(BaseGridObject obj) => claimedObjects.Remove(obj);

    public void SpawnFromSave()
    {
        var grid = Services.Get<Grid>();
        foreach (var saved in GameStore.Save.Hives)
        {
            var hive = grid.GetObjectAt(new Vector2I(saved.X, saved.Y)) as Hive;
            if (hive == null)
                continue;
            foreach (var kv in saved.BeeCounts)
                for (int i = 0; i < kv.Value; i++)
                    SpawnBee(kv.Key, hive);
        }
    }

    public override void _Ready()
    {
        SignalBus.Instance.GridLoaded += SpawnFromSave;
        SignalBus.Instance.GridObjectRemoved += OnGridObjectRemoved;
    }

    public override void _Process(double delta)
    {
        Grid grid = Services.Get<Grid>()!;
        Flower[] pollinatedFlowers = grid.GetObjectsOfType<Flower>()
            .Where(f =>
                (f.CurState == Flower.State.Pollinated) && (!IsClaimed(f)) && !(f is Blackhole)
            )
            .ToArray();
        Flower[] unpollinatedFlowers = grid.GetObjectsOfType<Flower>()
            .Where(f => f.CurState == Flower.State.Pollinating && !IsClaimed(f))
            .ToArray();

        foreach (var bee in GetIdleBees())
        {
            IBeeJob? job = bee.SelectJob(bee, pollinatedFlowers, unpollinatedFlowers);
            if (job != null && !(job is IdleJob))
                bee.SetJob(job);
        }

        // remove invalid objects
        foreach (BaseGridObject obj in claimedObjects.Keys)
        {
            if (!IsInstanceValid(claimedObjects[obj]))
                claimedObjects.Remove(obj);
        }
    }

    private void OnGridObjectRemoved(BaseGridObject obj)
    {
        if (!claimedObjects.TryGetValue(obj, out var bee))
            return;
        claimedObjects.Remove(obj);
        bee.SetJob(new IdleJob());
    }

    /// Spawn a bee by scene name (used by save load).
    public Bee? SpawnBee(string sceneName, Hive home)
    {
        var scene = GD.Load<PackedScene>($"res://objects/bees/{sceneName}.tscn");
        if (scene == null)
        {
            GD.PushError($"[BeeSystem] Missing bee scene: {sceneName}");
            return null;
        }
        return SpawnBee(scene, home, out FailMessage? failMessage);
    }

    /// Spawn a bee by scene (used by placement system).
    public Bee? SpawnBee(
        PackedScene scene,
        Hive home,
        out FailMessage? failMessage,
        bool travelToHive = false
    )
    {
        if (home.BeeCount >= GameStore.HiveCapacityBee.Value)
        {
            failMessage = new FailMessage(
                $"Hive at {home.GridPosition} is full ({home.BeeCount} bees)",
                "Hive is full!"
            );
            return null;
        }
        var bee = scene.Instantiate<Bee>();
        GetParent().AddChild(bee);
        bee.Setup(home);
        if (travelToHive)
        {
            bee.Visible = true;
            bee.SetJob(new GoToHiveJob());
            bee.GlobalPosition = bee.GetGlobalMousePosition();
            bee.Sprite.FlipH = bee.GlobalPosition.X < bee.Home.GlobalPosition.X;
        }
        EmitSignal(SignalName.BeeSpawned, bee);
        failMessage = null;
        return bee;
    }

    public Bee? SpawnBeeAnywhere(PackedScene scene)
    {
        Hive[] hives = Services.Get<Grid>().GetObjectsOfType<Hive>();
        if (hives.Length == 0)
            return null;
        return SpawnBee(scene, Utils.GetRandom(hives), out FailMessage? failMessage);
    }

    public bool RemoveBee(Type beeType, Hive hive)
    {
        Bee bee = GetBees().FirstOrDefault(b => b.GetType() == beeType && b.Home == hive)!;
        if (bee == null)
            return false;

        // release any object this bee has claimed
        var claimed = claimedObjects.Where(kv => kv.Value == bee).Select(kv => kv.Key).ToArray();
        foreach (var obj in claimed)
            claimedObjects.Remove(obj);

        hive.RemoveBee(bee);
        bee.Remove();
        EmitSignal(SignalName.BeeRemoved);
        return true;
    }

    public Bee[] GetBees()
    {
        List<Bee> bees = new List<Bee>();
        foreach (Node node in GetTree().GetNodesInGroup("bees"))
            if (node is Bee bee)
                bees.Add(bee);
        return bees.ToArray();
    }

    public int GetBeeCount() => GetBees().Length;

    public int GetBeeCountOfType(Type t) => GetBees().Count(b => b.GetType() == t);

    public Bee[] GetIdleBees() => GetBees().Where(b => b.job is IdleJob).ToArray();

    public Bee[] GetBeesWithJob<T>()
        where T : IBeeJob => GetBees().Where(b => b.job is T).ToArray();
}
