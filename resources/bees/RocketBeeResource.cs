using Godot;

[GlobalClass]
public partial class RocketBeeResource : BeeResource
{
    public override void ApplyStats(BeeEntity bee)
    {
        bee.Speed = new Stat(() => GameStore.BeeSpeed.Value);
        bee.BopSpeed = 10f;
    }

    public override IBeeJob HarvestJob() => new RocketHarvesterJob();

    public override IBeeJob PollinateJob() => new RocketPollinatorJob();
}
