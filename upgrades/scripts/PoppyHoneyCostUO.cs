using Godot;

[GlobalClass]
public partial class PoppyHoneyCostUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    public override string GetTechnicalText() => 
        $"{Style.CK("Poppy Honey Cost", "noun_poppy")} {Style.NumberChange(GameStore.PoppyHoneyCost.Value, GameStore.PoppyHoneyCost.Value + IncreaseBy)}";

    public override void Apply()
    {
        GameStore.PoppyHoneyCost.AddFlat(Name, IncreaseBy * Level);
    }
}
