using Godot;

[GlobalClass]
public partial class RocketBeeCapacityHoneyPerFatBeeBonusUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Rocket bees", "noun_rocket")} carry +{Style.NumberChange(GameStore.RocketBeeCapacityHoneyPerFatBeeBonus.Value, GameStore.RocketBeeCapacityHoneyPerFatBeeBonus.Value + IncreaseBy)} honey per {Style.CK("Fat bee", "noun_fat")}";

    public override void Apply() =>
        GameStore.RocketBeeCapacityHoneyPerFatBeeBonus.AddFlat(Name, IncreaseBy * Level);
}
