using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class BeeSystem : GameSystem
{
    public int BeeCount = 0;
    private PackedScene beeScene;
    private HashSet<Node> claimedTiles = new();

    public bool IsClaimed(Node tile) => claimedTiles.Contains(tile);

    public bool TryClaimTile(Node tile) => claimedTiles.Add(tile);

    public void ReleaseTile(Node tile) => claimedTiles.Remove(tile);

    public override void _Ready()
    {
        beeScene = GD.Load<PackedScene>("res://objects/Bee.tscn");

        Callable
            .From(() =>
            {
                var hive = Services.Get<Grid>().GetClosestTileOfType<HiveTile>(Vector2.Zero);
                for (int i = 0; i < 10; i++)
                {
                    Bee bee = SpawnBee(hive);
                    bee.Position = hive.GlobalPosition;
                    if (i % 2 == 0)
                        bee.SetJob(new PollinatorJob());
                    else
                        bee.SetJob(new HarvesterJob());
                }
            })
            .CallDeferred();
    }

    public override void _Process(double delta) { }

    public Bee SpawnBee(HiveTile home)
    {
        Bee bee = beeScene.Instantiate<Bee>();
        bee.Home = home;
        AddChild(bee);
        return bee;
    }

    public Bee[] GetBees()
    {
        var nodes = GetTree().GetNodesInGroup("bees");
        Bee[] bees = new Bee[nodes.Count];
        for (int i = 0; i < nodes.Count; i++)
            bees[i] = nodes[i] as Bee;
        return bees;
    }
}
