using Godot;

[GlobalClass]
public partial class CloverRegularHoneyGainUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Clover Base Yield", "noun_clover")} {Style.NumberChange(GameStore.CloverRegularHoneyGain.Value, GameStore.CloverRegularHoneyGain.Value + IncreaseBy)}";

    public override void Apply()
    {
        GameStore.CloverRegularHoneyGain.AddFlat(Name, IncreaseBy * Level);
    }
}
