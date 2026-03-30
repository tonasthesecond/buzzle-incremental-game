using Godot;

[GlobalClass]
public partial class QueenBeeResource : BeeResource
{
    public override void ApplyStats(BeeEntity bee) =>
        bee.Speed = new Stat(() => GameStore.BeeSpeed.Value * 0.8f);

    public override bool ReceivesWorkJobs => false;

    public override IBeeJob SpawnJob() => new QueenJob();
}
