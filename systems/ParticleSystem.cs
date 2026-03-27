using Godot;

[GlobalClass]
public partial class ParticleSystem : GameSystem
{
    private static readonly PackedScene honeyDripScene = GD.Load<PackedScene>("uid://qnqig8ge2b4q");

    public override void _Ready() => Services.Register(this);

    /// Attach a honey drip emitter to a node, returns the instance.
    public CpuParticles2D AttachHoneyDrip(Node2D target)
    {
        var particles = honeyDripScene.Instantiate<CpuParticles2D>();
        particles.Emitting = false;
        target.AddChild(particles);
        return particles;
    }
}
