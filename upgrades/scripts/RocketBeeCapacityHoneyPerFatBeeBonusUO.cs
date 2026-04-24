using Godot;

[GlobalClass]
public partial class RocketBeeCapacityHoneyPerFatBeeBonusUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Jetpack bees", "noun_rocket")} carry +{Style.NC(GameStore.RocketBeeCapacityHoneyPerFatBeeBonus.Value, GameStore.RocketBeeCapacityHoneyPerFatBeeBonus.Value + IncreaseBy, showChange: !IsMaxLevel())} honey per {Style.CK("Fat bee", "noun_fat")} in hive";

    public override void Apply() => GameStore.RocketBeeCapacityHoneyPerFatBeeBonus.AddFlat(Name, IncreaseBy * Level);
}
