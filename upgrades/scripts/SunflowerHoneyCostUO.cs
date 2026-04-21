using Godot;

[GlobalClass]
public partial class SunflowerHoneyCostUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Sunflower Honey Cost", "noun_sunflower")} {Style.NumberChange(GameStore.SunflowerHoneyCost.Value, GameStore.SunflowerHoneyCost.Value + IncreaseBy)}";

    public override void Apply()
    {
        GameStore.SunflowerHoneyCost.AddFlat(Name, IncreaseBy * Level);
    }
}
