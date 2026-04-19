using Godot;

[GlobalClass]
public partial class SunflowerHoneyGainUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    public override string GetTechnicalText() => 
        $"{Style.CK("Sunflower Honey Yield", "noun_sunflower")} {Style.NumberChange(GameStore.SunflowerHoneyGain.Value, GameStore.SunflowerHoneyGain.Value + IncreaseBy)}";

    public override void Apply()
    {
        GameStore.SunflowerHoneyGain.AddFlat(Name, IncreaseBy * Level);
    }
}
