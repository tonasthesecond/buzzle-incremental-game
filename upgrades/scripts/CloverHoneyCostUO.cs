using Godot;

[GlobalClass]
public partial class CloverHoneyCostUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    public override string GetTechnicalText() => 
        $"{Style.CK("Clover Honey Cost", "noun_clover")} {Style.NumberChange(GameStore.CloverHoneyCost.Value, GameStore.CloverHoneyCost.Value + IncreaseBy)}";

    public override void Apply()
    {
        GameStore.CloverHoneyCost.AddFlat(Name, IncreaseBy * Level);
    }
}
