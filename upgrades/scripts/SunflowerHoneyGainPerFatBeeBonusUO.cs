using Godot;

[GlobalClass]
public partial class SunflowerHoneyGainPerFatBeeBonusUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Sunflowers", "noun_sunflower")} gain +{Style.NC(GameStore.SunflowerHoneyGainPerFatBeeBonus.Value, GameStore.SunflowerHoneyGainPerFatBeeBonus.Value + IncreaseBy, showChange: !IsMaxLevel())} honey per {Style.CK("Fat bee", "noun_fat")} in hive";

    public override void Apply() => GameStore.SunflowerHoneyGainPerFatBeeBonus.AddFlat(Name, IncreaseBy * Level);
}
