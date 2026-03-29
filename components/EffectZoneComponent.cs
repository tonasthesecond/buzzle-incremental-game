using Godot;

public abstract partial class EffectZoneComponent : Area2D
{
    public Stat Radius { get; set; } = new(() => GameStore.BeekeeperRadius.Value);

    [Export]
    public float FadeoutTime = 1f;

    protected CollisionShape2D collisionShape;

    protected bool active = false;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;

        Deactivate();
        Hide();

        collisionShape = GetNode<CollisionShape2D>("%CollisionShape");
        collisionShape.Shape = new CircleShape2D();
        (collisionShape.Shape as CircleShape2D).Radius = Radius.Value;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Bee bee && active)
            OnBeeEntered(bee);
    }

    private void OnBodyExited(Node2D body)
    {
        if (body is Bee bee)
            OnBeeExited(bee);
    }

    public void Activate()
    {
        active = true;
        Show();
        // apply to bees already inside
        foreach (var body in GetOverlappingBodies())
            if (body is Bee bee)
                OnBeeEntered(bee);
    }

    public async void Deactivate()
    {
        active = false;
        foreach (var body in GetOverlappingBodies())
            if (body is Bee bee)
                scheduleRemove(bee);

        await ToSignal(GetTree().CreateTimer(FadeoutTime), SceneTreeTimer.SignalName.Timeout);
        Hide();
    }

    private async void scheduleRemove(Bee bee)
    {
        await ToSignal(GetTree().CreateTimer(FadeoutTime), SceneTreeTimer.SignalName.Timeout);
        if (IsInstanceValid(bee))
            OnBeeExited(bee);
    }

    protected abstract void OnBeeEntered(Bee bee);
    protected abstract void OnBeeExited(Bee bee);
}
