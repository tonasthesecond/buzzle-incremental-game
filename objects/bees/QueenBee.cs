using Godot;

[GlobalClass]
public partial class QueenBee : Bee
{
    public override IBeeJob SpawnJob() => new QueenJob();

    public override IBeeJob HarvestJob() => new QueenJob();

    public override IBeeJob PollinateJob() => new QueenJob();

    public override void ApplyStats(Bee bee) =>
        bee.Speed = new Stat(() => GameStore.BeeSpeed.Value * 0.8f);
}
