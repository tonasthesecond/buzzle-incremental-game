using Godot;

[GlobalClass]
public partial class PoppyHoneyGainUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    public override string GetHoverDescription() => 
        $"{Style.CK("Poppy Honey Yield", "noun_poppy")} {Style.NumberChange(GameStore.PoppyHoneyGain.Value, GameStore.PoppyHoneyGain.Value + IncreaseBy)}";

    public override void Apply()
    {
        GameStore.PoppyHoneyGain.AddFlat(Name, IncreaseBy * Level);
    }
}
