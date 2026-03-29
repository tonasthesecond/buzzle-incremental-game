using Godot;

public partial class QueenBeeEffectZoneComponent : EffectZoneComponent
{
    private string Key => "QueenBee";

    protected override void OnBeeEntered(Bee bee)
    {
        if (bee is QueenBee)
            return;

        bee.Speed.AddPercent(Key, GameStore.QueenBeeEffectZoneSpeedBuff.Value);
    }

    protected override void OnBeeExited(Bee bee) => bee.Speed.Remove(Key);

    public override void _Ready()
    {
        base._Ready();
        Radius = new(() => GameStore.QueenBeeEffectZoneRadius.Value);
        (collisionShape.Shape as CircleShape2D).Radius = Radius.Value;
        GameStore.QueenBeeEffectZoneRadius.Changed += () =>
            (collisionShape.Shape as CircleShape2D).Radius = Radius.Value;
    }

    public override void _Process(double delta)
    {
        // follow the queen's parent bee
        if (GetParent() is Bee queen)
            GlobalPosition = queen.GlobalPosition;
    }
}
