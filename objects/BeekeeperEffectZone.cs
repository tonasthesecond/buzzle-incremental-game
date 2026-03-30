using Godot;

public partial class BeekeeperEffectZone : EffectZoneComponent
{
    private string Key => $"Beekeeper";

    // bees entering the zone get a speed buff
    protected override void OnBeeEntered(BeeEntity bee) =>
        bee.Speed.AddPercent(Key, GameStore.BeekeeperEffectZoneSpeedBuff.Value);

    // bees leaving the zone lose the speed buff
    protected override void OnBeeExited(BeeEntity bee) => bee.Speed.Remove(Key);

    public override void _Ready()
    {
        base._Ready();

        // collision shape updates using the radius stat
        Radius = new(() => GameStore.BeekeeperEffectZoneRadius.Value);
        (collisionShape.Shape as CircleShape2D).Radius = Radius.Value;
        GameStore.BeekeeperEffectZoneRadius.Changed += () =>
            (collisionShape.Shape as CircleShape2D).Radius = Radius.Value;

        // fadeout time updates using the fadeout time stat
        FadeoutTime = new(() => GameStore.BeekeeperEffectZoneFadeoutTime.Value);
    }

    public override void _Process(double delta)
    {
        // follow mouse when holding
        if (active && fadeTimer.IsStopped())
            GlobalPosition = GetGlobalMousePosition();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // don't activate when placing tiles or objects
        if (Services.Get<PlacementSystem>().CurMode != PlacementSystem.Mode.None)
            return;

        // activate on left click
        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true })
        {
            Activate();
            GetViewport().SetInputAsHandled();
        }
        // deactivate on release left click
        else if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: false })
        {
            Deactivate();
            GetViewport().SetInputAsHandled();
        }
    }
}
