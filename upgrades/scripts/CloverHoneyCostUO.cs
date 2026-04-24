using Godot;

[GlobalClass]
public partial class CloverHoneyCostUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Clovers", "noun_clover")} cost {Style.NC(GameStore.CloverHoneyCost.Value, GameStore.CloverHoneyCost.Value + IncreaseBy, showChange: !IsMaxLevel())} honey to pollinate";

    public override void Apply() => GameStore.CloverHoneyCost.AddFlat(Name, IncreaseBy * Level);
}
