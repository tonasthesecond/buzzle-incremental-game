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
                    bee.Position = hive.GlobalPosition;
                    if (i % 2 == 0)
                        bee.SetJob(new PollinatorJob());
                    else
                        bee.SetJob(new HarvesterJob());
                }
            })
            .CallDeferred();
    }

    // public override void _Process(double delta) { }

    public Bee? SpawnBee(HiveTile home)
    {
        Bee bee = beeScene.Instantiate<Bee>();
        bee.Home = home;
        AddChild(bee);
        return bee;
    }

    public Bee? SpawnBeeAnywhere()
    {
        HiveTile[] hives = Services.Get<Grid>().GetTilesOfType<HiveTile>();
        if (hives.Length == 0)
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
}
