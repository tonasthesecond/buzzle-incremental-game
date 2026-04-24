using Godot;

[GlobalClass]
public partial class PoppyHoneyCostUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Poppies", "noun_poppy")} cost {Style.NC(GameStore.PoppyHoneyCost.Value, GameStore.PoppyHoneyCost.Value + IncreaseBy, showChange: !IsMaxLevel())} honey to pollinate";

    public override void Apply() => GameStore.PoppyHoneyCost.AddFlat(Name, IncreaseBy * Level);
}
