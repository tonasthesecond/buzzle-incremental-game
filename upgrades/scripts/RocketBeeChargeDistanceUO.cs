using Godot;

[GlobalClass]
public partial class RocketBeeChargeDistanceUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 5f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Jetpack bees", "noun_rocket")} pull back {Style.NC(GameStore.RocketBeeChargeDistance.Value, GameStore.RocketBeeChargeDistance.Value + IncreaseBy, showChange: !IsMaxLevel())} pixels before launching";

    public override void Apply() => GameStore.RocketBeeChargeDistance.AddFlat(Name, IncreaseBy * Level);
}
