using Godot;

[GlobalClass]
public partial class SunflowerHoneyGainUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Sunflowers", "noun_sunflower")} yield {Style.NC(GameStore.SunflowerHoneyGain.Value, GameStore.SunflowerHoneyGain.Value + IncreaseBy, showChange: !IsMaxLevel())} honey";

    public override void Apply() => GameStore.SunflowerHoneyGain.AddFlat(Name, IncreaseBy * Level);
}
