using Godot;

[GlobalClass]
public partial class HeatVaporParticles : GpuParticles2D
{
    [Export]
    public int BaseAmount = 32;

    [Export]
    public float ParticleLifetime = 2.4f;

    [Export]
    public float EmitRadius = 50f;

    [Export]
    public float SpeedMin = 10f;

    [Export]
    public float SpeedMax = 30f;

    [Export]
    public float ScaleMin = 1f;

    [Export]
    public float ScaleMax = 3f;

    [Export]
    public float ParticleSize = 16f;

    private ShaderMaterial _mat;

    public override void _Ready()
    {
        _mat = (ShaderMaterial)ProcessMaterial;
        Amount = BaseAmount;
        Lifetime = ParticleLifetime;
        Emitting = false;
        SyncUniforms();
    }

    private void SyncUniforms()
    {
        _mat.SetShaderParameter("emit_radius", EmitRadius);
        _mat.SetShaderParameter("speed_min", SpeedMin);
        _mat.SetShaderParameter("speed_max", SpeedMax);
        _mat.SetShaderParameter("scale_min", ScaleMin);
        _mat.SetShaderParameter("scale_max", ScaleMax);
        _mat.SetShaderParameter("particle_size", ParticleSize);
    }

    public void UpdateRadius(float radius)
    {
        EmitRadius = radius;
        _mat?.SetShaderParameter("emit_radius", radius);
    }

    public void SetHeatLevel(float heat)
    {
        AmountRatio = Mathf.Clamp(heat, 0f, 1f);
    }
}
