using Godot;

[GlobalClass]
public partial class RoseHoneyGainUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    public override string GetHoverDescription() => 
        $"{Style.CK("Rose Honey Yield", "noun_rose")} {Style.NumberChange(GameStore.RoseHoneyGain.Value, GameStore.RoseHoneyGain.Value + IncreaseBy)}";

    public override void Apply()
    {
        GameStore.RoseHoneyGain.AddFlat(Name, IncreaseBy * Level);
    }
}
