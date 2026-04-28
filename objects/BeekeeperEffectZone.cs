using Godot;

public partial class BeekeeperEffectZone : EffectZoneComponent
{
    private string Key => "Beekeeper";

    protected override void OnBeeEntered(Bee bee)
    {
        bee.Speed.AddPercent(Key, GameStore.BeekeeperEffectZoneSpeedBuff.Value);
        bee.PollinationTimeReductionBuff.AddFlat(
            Key,
            GameStore.BeekeeperEffectZonePollinationTimeReductionBuff.Value
        );
    }

    protected override void OnBeeExited(Bee bee) => bee.Speed.Remove(Key);

    public override void _Ready()
    {
        base._Ready();
        Radius = new(() => GameStore.BeekeeperEffectZoneRadius.Value);
        FadeoutTime = new(() => GameStore.BeekeeperEffectZoneFadeoutTime.Value);
        (collisionShape.Shape as CircleShape2D).Radius = Radius.Value;
        GameStore.BeekeeperEffectZoneRadius.Changed += () =>
            (collisionShape.Shape as CircleShape2D).Radius = Radius.Value;
        fadeTimer.Timeout += () =>
        {
            heatVapor.Emitting = false;
        };
    }

    public override void _Process(double delta)
    {
        if (active && fadeTimer.IsStopped() && !GameStore.BeekeeperEffectZoneNeverFade)
            GlobalPosition = GetGlobalMousePosition();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Services.Get<PlacementSystem>().CurMode != PlacementSystem.Mode.None)
            return;
        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true })
        {
            Activate();
            UpdateHeatVapor();
            heatVapor.Emitting = true;
            GetViewport().SetInputAsHandled();
        }
        else if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: false })
        {
            if (GameStore.BeekeeperEffectZoneNeverFade)
                return;
            Deactivate();
            GetViewport().SetInputAsHandled();
        }
    }

    private void UpdateHeatVapor()
    {
        heatVapor.SetHeatLevel(GameStore.BeekeeperEffectZoneSpeedBuff.Value);
        heatVapor.UpdateRadius(Radius.Value);
    }
}
