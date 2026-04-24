using Godot;

[GlobalClass]
public partial class CloverRegularHoneyGainUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Clovers", "noun_clover")} yield {Style.NC(GameStore.CloverRegularHoneyGain.Value, GameStore.CloverRegularHoneyGain.Value + IncreaseBy, showChange: !IsMaxLevel())} honey normally";

    public override void Apply() => GameStore.CloverRegularHoneyGain.AddFlat(Name, IncreaseBy * Level);
}
