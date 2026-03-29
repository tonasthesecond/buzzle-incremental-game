using Godot;

public partial class BeekeeperEffectZone : EffectZoneComponent
{
    private string Key => $"Beekeeper";

    protected override void OnBeeEntered(Bee bee) =>
        bee.Speed.AddPercent(Key, GameStore.BeekeeperSpeedBuff.Value);

    protected override void OnBeeExited(Bee bee) => bee.Speed.Remove(Key);

    public override void _Process(double delta)
    {
        if (active)
            GlobalPosition = GetGlobalMousePosition();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Services.Get<PlacementSystem>().CurMode != PlacementSystem.Mode.None)
            return;

        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true })
        {
            Activate();
            GetViewport().SetInputAsHandled();
        }
        else if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: false })
        {
            Deactivate();
            GetViewport().SetInputAsHandled();
        }
    }
}
