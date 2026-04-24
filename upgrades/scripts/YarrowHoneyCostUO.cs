using Godot;

[GlobalClass]
public partial class YarrowHoneyCostUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Yarrows", "noun_yarrow")} cost {Style.NC(GameStore.YarrowHoneyCost.Value, GameStore.YarrowHoneyCost.Value + IncreaseBy, showChange: !IsMaxLevel())} honey to pollinate";

    public override void Apply() => GameStore.YarrowHoneyCost.AddFlat(Name, IncreaseBy * Level);
}
