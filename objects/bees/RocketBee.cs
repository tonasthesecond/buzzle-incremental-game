using Godot;

[GlobalClass]
public partial class RocketBee : Bee
{
    public override void ApplyStats(Bee bee)
    {
        bee.Speed = new Stat(() => GameStore.BeeSpeed.Value);
        bee.BopSpeed = 10f;
    }

    public override IBeeJob HarvestJob() =>
        GameStore.RocketBeeIsolatedHarvest
            ? new RocketIsolatedHarvesterJob()
            : new RocketHarvesterJob();

    public override IBeeJob PollinateJob() => new RocketPollinatorJob();
}
