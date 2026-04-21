using Godot;

[GlobalClass]
public partial class PlacementExplosionParticles : GpuParticles2D
{
    [Export]
    public float BurstSpeedMin = 50f;

    [Export]
    public float BurstSpeedMax = 100f;

    [Export]
    public float Damping = 3f;

    [Export]
    public float ParticleSize = 1f;

    [Export]
    public float LingerStart = 0.6f;

    [Export]
    public float Gravity = 200f;

    private ShaderMaterial _mat;

    public override void _Ready()
    {
        _mat = (ShaderMaterial)ProcessMaterial;
        OneShot = true;
        Emitting = false;
        SyncUniforms();
    }

    private void SyncUniforms()
    {
        _mat.SetShaderParameter("burst_speed_min", BurstSpeedMin);
        _mat.SetShaderParameter("burst_speed_max", BurstSpeedMax);
        _mat.SetShaderParameter("damping", Damping);
        _mat.SetShaderParameter("particle_size", ParticleSize);
        _mat.SetShaderParameter("linger_start", LingerStart);
        _mat.SetShaderParameter("gravity", Gravity);
    }

    public void Emit(Vector2 position)
    {
        GlobalPosition = position;
        Restart();
    }
}
