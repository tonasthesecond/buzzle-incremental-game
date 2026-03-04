using Godot;

public partial class BeeSystem : Node
{
    public int BeeCount = 0;
    public int BeeSpeed = 200;
    private PackedScene beeScene;

    public override void _Ready()
    {
        beeScene = GD.Load<PackedScene>("res://objects/bee.tscn");
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
                bee.targetPosition = new Vector2(
                    (float)GD.Randf() * GetViewport().GetVisibleRect().Size.X,
                    (float)GD.Randf() * GetViewport().GetVisibleRect().Size.Y
                );
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
