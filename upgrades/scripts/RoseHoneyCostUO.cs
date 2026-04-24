using Godot;

[GlobalClass]
public partial class RoseHoneyCostUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Roses", "noun_rose")} cost {Style.NC(GameStore.RoseHoneyCost.Value, GameStore.RoseHoneyCost.Value + IncreaseBy, showChange: !IsMaxLevel())} honey to pollinate";

    public override void Apply() => GameStore.RoseHoneyCost.AddFlat(Name, IncreaseBy * Level);
}
