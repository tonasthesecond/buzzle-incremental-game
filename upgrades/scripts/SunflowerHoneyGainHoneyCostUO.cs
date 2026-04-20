using Godot;

[GlobalClass]
public partial class SunflowerHoneyGainHoneyCostUO : IUpgradeOption
{
    [Export]
    public float IncreaseGainBy { get; set; } = 2f;

    [Export]
    public float IncreaseCostBy { get; set; } = 1f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Sunflowers", "noun_sunflower")} produce {Style.NumberChange(GameStore.SunflowerHoneyGain.Value, GameStore.SunflowerHoneyGain.Value + IncreaseGainBy)} honey\n{Style.CK("Sunflowers", "noun_sunflower")} cost {Style.NumberChange(GameStore.SunflowerHoneyCost.Value, GameStore.SunflowerHoneyCost.Value + IncreaseCostBy)} honey";

    public override void Apply()
    {
        GameStore.SunflowerHoneyGain.AddFlat(Name, IncreaseGainBy * Level);
        GameStore.SunflowerHoneyCost.AddFlat(Name, IncreaseCostBy * Level);
    }
}
