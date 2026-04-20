using Godot;

[GlobalClass]
public partial class YarrowHoneyCostUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    public override string GetTechnicalText() => 
        $"{Style.CK("Yarrow Honey Cost", "noun_yarrow")} {Style.NumberChange(GameStore.YarrowHoneyCost.Value, GameStore.YarrowHoneyCost.Value + IncreaseBy)}";

    public override void Apply()
    {
        GameStore.YarrowHoneyCost.AddFlat(Name, IncreaseBy * Level);
    }
}
