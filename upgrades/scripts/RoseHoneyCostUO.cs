using Godot;

[GlobalClass]
public partial class RoseHoneyCostUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    public override string GetTechnicalText() => 
        $"{Style.CK("Rose Honey Cost", "noun_rose")} {Style.NumberChange(GameStore.RoseHoneyCost.Value, GameStore.RoseHoneyCost.Value + IncreaseBy)}";

    public override void Apply()
    {
        GameStore.RoseHoneyCost.AddFlat(Name, IncreaseBy * Level);
    }
}
