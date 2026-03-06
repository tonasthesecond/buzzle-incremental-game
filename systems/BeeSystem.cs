using Godot;

[GlobalClass]
public partial class BeeSystem : GameSystem
{
    public int BeeCount = 0;
    private PackedScene beeScene;

    public override void _Ready()
    {
        beeScene = GD.Load<PackedScene>("res://objects/Bee.tscn");
        // TEST: spawn 10 bees
        for (int i = 0; i < 10; i++)
        {
            SpawnBee();
        }
    }

    public override void _Process(double delta)
    {
        // TEST: give bees a random target position
        foreach (Bee bee in GetBees())
        {
            if (bee.state == Bee.State.Idling)
            {
                int randomX = (int)GD.RandRange(0, Services.Get<Grid>().Width - 1);
                int randomY = (int)GD.RandRange(0, Services.Get<Grid>().Height - 1);
                bee.targetPosition = Services
                    .Get<Grid>()
                    .GridToWorld(new Vector2I(randomX, randomY));
                bee.state = Bee.State.Moving;
            }
        }
    }

    /// <summary>
    /// Spawns a new bee in the game.
    /// </summary>
    public void SpawnBee()
    {
        Bee bee = beeScene.Instantiate() as Bee;
        AddChild(bee);
    }

    /// <summary>
    /// Returns an array of all bees in the game.
    /// </summary>
    public Bee[] GetBees()
    {
        var nodes = GetTree().GetNodesInGroup("bees");
        Bee[] bees = new Bee[nodes.Count];
        for (int i = 0; i < nodes.Count; i++)
        {
            bees[i] = nodes[i] as Bee;
        }
        return bees;
    }
}
