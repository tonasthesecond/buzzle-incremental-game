using Godot;

[GlobalClass]
public partial class SunflowerHoneyGainPerFatBeeBonusUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Sunflowers", "noun_sunflower")} produce +{Style.NC(GameStore.SunflowerHoneyGainPerFatBeeBonus.Value, GameStore.SunflowerHoneyGainPerFatBeeBonus.Value + IncreaseBy)} extra honey per {Style.CK("Fat bee", "noun_fat")}";

    public override void Apply() =>
        GameStore.SunflowerHoneyGainPerFatBeeBonus.AddFlat(Name, IncreaseBy * Level);
}
