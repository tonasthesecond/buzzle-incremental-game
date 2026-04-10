using Godot;

[GlobalClass]
public abstract partial class Bee : CharacterBody2D
{
    [Signal]
    public delegate void ArrivedEventHandler(Bee bee);

    [Export]
    public string BeeTypeName { get; set; } = "regular";

    public IBeeJob job = new IdleJob();
    public required Hive Home;
    public bool IsAnimating { get; private set; }
    public int carryingHoney = 0;
    public Vector2 targetPosition;
    public Stat Speed = new(() => GameStore.BeeSpeed.Value);
    public Stat HoneyCapacity = new(() => GameStore.BeeCapacityHoney.Value);
    public bool IsMoving =>
        Position.DistanceSquaredTo(targetPosition) >= Mathf.Pow(GameStore.TILE_SIZE / 20f, 2);
    private float phase;
    public float BopAmplitude = 1.5f;
    public float BopSpeed = 3f;
    public bool? FlipOverride = null;
    private float spriteY = 0f;

    // cached services
    private Grid grid = null!;
    private BeeSystem beeSystem = null!;
    public Sprite2D Sprite = null!;
    private CpuParticles2D drip = null!;

    /// Apply stat modifiers to the bee on spawn.
    public virtual void ApplyStats(Bee bee) { }

    /// Job to assign when the bee should harvest.
    public virtual IBeeJob HarvestJob() => new HarvesterJob();

    /// Job to assign when the bee should pollinate.
    public virtual IBeeJob PollinateJob() => new PollinatorJob();

    /// Initial job on spawn - override to start with something other than IdleJob.
    public virtual IBeeJob SpawnJob() => new IdleJob();

    public virtual IBeeJob? SelectJob(
        Bee bee,
        Flower[] pollinatedFlowers,
        Flower[] unpollinatedFlowers
    )
    {
        BeeSystem beeSystem = Services.Get<BeeSystem>()!;
        int harvesters = beeSystem.GetBeesWithJob<HarvesterJob>().Length;
        int pollinators = beeSystem.GetBeesWithJob<PollinatorJob>().Length;
        if (harvesters < pollinatedFlowers.Length)
            return HarvestJob();
        if (pollinators < unpollinatedFlowers.Length)
            return PollinateJob();
        return null;
    }

    public void MoveTo(Vector2 position) => targetPosition = position;

    public void SetJob(IBeeJob newJob) => job = newJob;

    // orbit state for pollination animation
    private Vector2 orbitCenter;
    private float orbitAngle;
    private float orbitDir;
    private float orbitRadius;
    private float orbitSpeedMult = 1.2f;
    private Vector2 centerOffset = new Vector2(0, -4);
    private bool latchedFlipH;

    /// Orbit the sprite around a center point for the pollination animation.
    public void StartPollinatingAnim(Vector2 center, float duration, float radius = 15f)
    {
        IsAnimating = true;
        orbitCenter = center + centerOffset;
        orbitRadius = radius;
        bool clockwise = orbitCenter.X >= GlobalPosition.X;
        orbitDir = clockwise ? 1f : -1f;
        orbitAngle = (GlobalPosition - center).Angle();

        var tween = CreateTween();
        tween.TweenMethod(
            Callable.From(
                (float t) =>
                {
                    orbitAngle =
                        (GlobalPosition - orbitCenter).Angle()
                        + orbitDir * (Speed.Value / orbitRadius) * (float)GetProcessDeltaTime();
                }
            ),
            0f,
            1f,
            duration
        );
        tween.Finished += () =>
        {
            IsAnimating = false;
            Sprite.FlipH = latchedFlipH;
        };
    }

    public override void _Ready()
    {
        phase = GD.Randf() * Mathf.Tau;
        Sprite = GetNode<Sprite2D>("Sprite2D")!;
        grid = Services.Get<Grid>()!;
        beeSystem = Services.Get<BeeSystem>()!;
        drip = Services.Get<ParticleSystem>().AttachHoneyDrip(this);
        Callable.From(() => targetPosition = Position).CallDeferred();
    }

    public override void _Process(double delta)
    {
        job.Tick(this);

        // drip whenever carrying honey
        drip.Emitting = carryingHoney > 0;
        int targetAmount = carryingHoney + 1;
        if (drip.Amount != targetAmount)
            drip.Amount = targetAmount;

        if (IsAnimating)
        {
            orbitAngle += orbitDir * (Speed.Value / orbitRadius) * orbitSpeedMult * (float)delta;
            var orbitTarget =
                orbitCenter
                + new Vector2(Mathf.Cos(orbitAngle), Mathf.Sin(orbitAngle)) * orbitRadius;
            GlobalPosition = GlobalPosition.MoveToward(
                orbitTarget,
                Speed.Value * orbitSpeedMult * (float)delta
            );
            Sprite.FlipH = orbitDir < 0 ? Mathf.Sin(orbitAngle) < 0 : Mathf.Sin(orbitAngle) > 0;
            latchedFlipH = Sprite.FlipH;
            float orbitTargetY = 1.5f * Mathf.Sin(orbitAngle * 2f);
            spriteY = Mathf.Lerp(spriteY, orbitTargetY, (float)delta * 10f);
            Sprite.Position = new Vector2(0, spriteY);
        }
        else
        {
            // always bob so there's no snap on landing
            Sprite.FlipH = latchedFlipH;
            if (IsMoving)
                Move(delta);
            float bobTargetY = Mathf.Sin(phase) * BopAmplitude;
            spriteY = Mathf.Lerp(spriteY, bobTargetY, (float)delta * 10f);
            Sprite.Position = new Vector2(Sprite.Position.X, spriteY);
        }
        phase += (float)delta * BopSpeed;
    }

    /// Initialize bee at home hive.
    public void Setup(Hive home)
    {
        Home = home;
        home.AddBee(this);
        ApplyStats(this);
        GlobalPosition = Home.GlobalPosition;
        SetJob(SpawnJob());
        Modulate = Colors.Transparent;
        Visible = false;
        _fadeTarget = 0f;
    }

    /// Move toward target position.
    void Move(double delta)
    {
        Position = Position.MoveToward(targetPosition, Speed.Value * (float)delta);
        latchedFlipH = FlipOverride ?? (targetPosition.X < Position.X);
        Sprite.FlipH = latchedFlipH;
    }

    private float _fadeTarget = 0f;
    private Tween? fadeTween;

    /// Tween the bee's opacity.
    public void FadeTo(float alpha, float duration = 0.3f)
    {
        if (Mathf.IsEqualApprox(_fadeTarget, alpha))
            return;
        _fadeTarget = alpha;
        Visible = true;
        if (fadeTween != null)
            fadeTween.Kill();
        fadeTween = CreateTween();
        fadeTween.TweenProperty(this, "modulate:a", alpha, duration);
        if (alpha == 0f)
            fadeTween.TweenCallback(Callable.From(() => Visible = false));
    }
}
