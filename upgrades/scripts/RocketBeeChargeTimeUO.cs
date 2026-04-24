using Godot;

[GlobalClass]
public partial class RocketBeeChargeTimeUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -100f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Jetpack bee", "noun_rocket")} charge time {Style.NC(GameStore.RocketBeeChargeTime.Value, GameStore.RocketBeeChargeTime.Value + IncreaseBy, showChange: !IsMaxLevel())}ms";

    public override void Apply() => GameStore.RocketBeeChargeTime.AddFlat(Name, IncreaseBy * Level);
}
