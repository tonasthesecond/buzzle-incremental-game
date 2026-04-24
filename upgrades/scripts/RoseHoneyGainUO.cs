using Godot;

[GlobalClass]
public partial class RoseHoneyGainUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Roses", "noun_rose")} yield {Style.NC(GameStore.RoseHoneyGain.Value, GameStore.RoseHoneyGain.Value + IncreaseBy, showChange: !IsMaxLevel())} base honey";

    public override void Apply() => GameStore.RoseHoneyGain.AddFlat(Name, IncreaseBy * Level);
}
