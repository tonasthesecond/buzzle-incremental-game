#nullable enable
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class BeeSystem : GameSystem
{
    private PackedScene beeScene = GD.Load<PackedScene>("res://objects/Bee.tscn");
    private HashSet<BaseTile> claimedTiles = new();

    public bool IsClaimed(BaseTile tile) => claimedTiles.Contains(tile);

    public bool ClaimTile(BaseTile tile) => claimedTiles.Add(tile);

    public void ReleaseTile(BaseTile tile) => claimedTiles.Remove(tile);

    public override void _Ready()
    {
        Callable
            .From(() =>
            {
                var hive = Services.Get<Grid>().GetClosestTileOfType<HiveTile>(Vector2.Zero);
                for (int i = 0; i < GameStore.BeeCount; i++)
                {
                    Bee? bee = SpawnBee(hive);
                }
            })
            .CallDeferred();
    }

    public override void _Process(double delta) { }

    public Bee? SpawnBee(HiveTile home)
    {
        if (home.Bees.Count >= GameStore.HiveBeeCapacity)
        {
            // TODO: add error messages
            return null;
        }
        Bee bee = beeScene.Instantiate<Bee>();
        AddChild(bee);
        bee.Setup(home, getAlternateBeeJob());
        return bee;
    }

    public Bee? SpawnBeeAnywhere()
    {
        HiveTile[] hives = Services.Get<Grid>().GetTilesOfType<HiveTile>();
        if (hives.Length == 0)
            // TODO: add error messages
            return null;
        return SpawnBee(Utils.GetRandom(hives));
    }

    public Bee[] GetBees()
    {
        var nodes = GetTree().GetNodesInGroup("bees");
        List<Bee> bees = new();
        foreach (Bee node in nodes)
        {
            bees.Add(node);
        }
        return bees.ToArray();
    }

    private IBeeJob getAlternateBeeJob()
    {
        if (GetBees().Length % 2 == 0)
            return new HarvesterJob();
        else
            return new PollinatorJob();
    }
}
