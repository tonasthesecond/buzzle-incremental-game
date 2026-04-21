using Godot;

public partial class QueenBeeEffectZoneComponent : EffectZoneComponent
{
    private string Key => "QueenBee";

    protected override void OnBeeEntered(Bee bee)
    {
        if (bee is QueenBee)
            return;

        bee.Speed.AddPercent(Key, GameStore.QueenBeeEffectZoneSpeedBuff.Value);
        bee.PollinationTimeReductionBuff.AddFlat(
            Key,
            GameStore.QueenBeeEffectZonePollinationTimeReductionBuff.Value
        );
        heatVapor.Emitting = true;
        UpdateHeatVapor();
    }

    protected override void OnBeeExited(Bee bee)
    {
        bee.Speed.Remove(Key);
        bee.PollinationTimeReductionBuff.Remove(Key);
    }

    public override void _Ready()
    {
        base._Ready();
        Radius = new(() => GameStore.QueenBeeEffectZoneRadius.Value);
        (collisionShape.Shape as CircleShape2D).Radius = Radius.Value;
        Radius.Changed += () => UpdateHeatVapor();
        GameStore.QueenBeeEffectZoneRadius.Changed += () =>
            (collisionShape.Shape as CircleShape2D).Radius = Radius.Value;
        Activate();
    }

    public override void _Process(double delta)
    {
        // follow the queen's parent bee
        if (GetParent() is Bee queen)
            GlobalPosition = queen.GlobalPosition;
    }

    private void UpdateHeatVapor()
    {
        heatVapor.EmitRadius = Radius.Value;
    }
}
