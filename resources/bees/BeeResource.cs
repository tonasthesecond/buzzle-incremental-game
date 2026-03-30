using Godot;

[GlobalClass]
public abstract partial class BeeResource : Resource
{
    /// Apply stat modifiers to the bee on spawn.
    public virtual void ApplyStats(BeeEntity bee) { }

    /// Job to assign when the bee should harvest.
    public virtual IBeeJob HarvestJob() => new HarvesterJob();

    /// Job to assign when the bee should pollinate.
    public virtual IBeeJob PollinateJob() => new PollinatorJob();

    /// Initial job on spawn — override to start with something other than IdleJob.
    public virtual IBeeJob SpawnJob() => new IdleJob();

    /// Whether BeeSystem should assign work jobs to this bee.
    public virtual bool ReceivesWorkJobs => true;

    // BeeResource.cs
    public virtual IBeeJob? SelectJob(
        BeeEntity bee,
        BaseFlower[] withHoney,
        BaseFlower[] withoutHoney
    )
    {
        BeeSystem beeSystem = Services.Get<BeeSystem>()!;
        int harvesters = beeSystem.GetBeesWithJob<HarvesterJob>().Length;
        int pollinators = beeSystem.GetBeesWithJob<PollinatorJob>().Length;
        if (harvesters < withHoney.Length)
            return HarvestJob();
        if (pollinators < withoutHoney.Length)
            return PollinateJob();
        return null;
    }
}
