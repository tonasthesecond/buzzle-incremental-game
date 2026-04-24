using Godot;

[GlobalClass]
public partial class SunflowerHoneyGainHoneyCostUO : IUpgradeOption
{
    [Export]
    public float IncreaseGainBy { get; set; } = 2f;

    [Export]
    public float IncreaseCostBy { get; set; } = 1f;

    public override string GetTechnicalText()
    {
        bool show = !IsMaxLevel();
        return $"{Style.CK("Sunflowers", "noun_sunflower")} yield {Style.NC(GameStore.SunflowerHoneyGain.Value, GameStore.SunflowerHoneyGain.Value + IncreaseGainBy, showChange: show)} honey\n"
             + $"{Style.CK("Sunflowers", "noun_sunflower")} cost {Style.NC(GameStore.SunflowerHoneyCost.Value, GameStore.SunflowerHoneyCost.Value + IncreaseCostBy, showChange: show)} honey to pollinate";
    }

    public override void Apply()
    {
        GameStore.SunflowerHoneyGain.AddFlat(Name, IncreaseGainBy * Level);
        GameStore.SunflowerHoneyCost.AddFlat(Name, IncreaseCostBy * Level);
    }
}
