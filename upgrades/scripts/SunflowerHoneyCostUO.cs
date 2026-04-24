using Godot;

[GlobalClass]
public partial class SunflowerHoneyCostUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Sunflowers", "noun_sunflower")} cost {Style.NC(GameStore.SunflowerHoneyCost.Value, GameStore.SunflowerHoneyCost.Value + IncreaseBy, showChange: !IsMaxLevel())} honey to pollinate";

    public override void Apply() => GameStore.SunflowerHoneyCost.AddFlat(Name, IncreaseBy * Level);
}
