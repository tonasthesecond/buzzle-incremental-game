using Godot;

public partial class BeeEntity : Node2D
{
    [Signal]
    public delegate void ArrivedEventHandler(BeeEntity bee);

    [Export]
    public BeeResource Definition { get; private set; } = new BaseBeeResource();

    public IBeeJob job = new IdleJob();
    public required HiveGridObject Home;
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

    // cached services
    private Grid grid = null!;
    private BeeSystem beeSystem = null!;
    public Sprite2D Sprite = null!;
    private CpuParticles2D drip = null!;

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
    public void StartPollinatingAnim(Vector2 center, float duration = 3f, float radius = 15f)
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
        int targetAmount = carryingHoney + 2;
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
            Sprite.Position = new Vector2(0, 1.5f * Mathf.Sin(orbitAngle * 2f));
        }
        else
        {
            Sprite.FlipH = latchedFlipH;
            if (IsMoving)
                Move(delta);
            // always bob so there's no snap on landing
            Sprite.Position = new Vector2(Sprite.Position.X, Mathf.Sin(phase) * BopAmplitude);
        }
        phase += (float)delta * BopSpeed;
    }

    /// Initialize bee at home hive.
    public void Setup(HiveGridObject home)
    {
        Home = home;
        home.AddBee(this);
        Definition.ApplyStats(this);
        GlobalPosition = Home.GlobalPosition;
        SetJob(Definition.SpawnJob());
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
