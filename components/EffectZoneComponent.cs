using Godot;

public abstract partial class EffectZoneComponent : Area2D
{
    public Stat Radius { get; set; } = new(GameStore.TILE_SIZE / 2);
    public Stat FadeoutTime { get; set; } = new(1f);

    protected Timer fadeTimer;

    protected CollisionShape2D collisionShape;
    protected bool active = false;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;

        // collision shape
        collisionShape = GetNode<CollisionShape2D>("%CollisionShape");
        collisionShape.Shape = new CircleShape2D();
        (collisionShape.Shape as CircleShape2D).Radius = Radius.Value;

        // fade timer
        fadeTimer = new Timer { OneShot = true, WaitTime = FadeoutTime.Value };
        AddChild(fadeTimer);
        fadeTimer.Timeout += () =>
        {
            Hide();
            active = false;
            // call exited event for all bees in the zone
            foreach (var body in GetOverlappingBodies())
                if (body is BeeEntity bee)
                    OnBeeExited(bee);
        };

        // immediately hide
        Hide();
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is BeeEntity bee && active)
            OnBeeEntered(bee);
    }

    private void OnBodyExited(Node2D body)
    {
        if (body is BeeEntity bee)
            OnBeeExited(bee);
    }

    public void Activate()
    {
        fadeTimer.Stop();
        active = true;
        Show();
        foreach (var body in GetOverlappingBodies())
            if (body is BeeEntity bee)
                OnBeeEntered(bee);
    }

    public void Deactivate()
    {
        fadeTimer.Start();
    }

    protected abstract void OnBeeEntered(BeeEntity bee);
    protected abstract void OnBeeExited(BeeEntity bee);
}
