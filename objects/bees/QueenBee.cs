using Godot;

/// Wanders the hive area, applying passive effects. Never gets assigned a work job.
public partial class QueenBee : Bee
{
    private EffectZoneComponent queenEffectZone = null!;

    public override void _Ready()
    {
        base._Ready();
        // queen starts with her own wandering job, not IdleJob
        // Setup() assigns IdleJob first, so we override after _Ready via CallDeferred
        Callable.From(() => SetJob(new QueenJob())).CallDeferred();

        queenEffectZone = GetNode<EffectZoneComponent>("%EffectZoneComponent");
        queenEffectZone.Activate();
    }
}
