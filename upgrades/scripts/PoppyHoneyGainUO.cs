using Godot;

[GlobalClass]
public partial class PoppyHoneyGainUO : IUpgradeOption
{
    [Export]
    public int IncreaseBy { get; set; } = 1;

    public override string GetTechnicalText() =>
        $"{Style.CK("Poppies", "noun_poppy")} produce {Style.NumberChange(GameStore.PoppyHoneyGain.Value, GameStore.PoppyHoneyGain.Value + IncreaseBy)} honey.";

    public override void Apply() => GameStore.PoppyHoneyGain.AddFlat(Name, IncreaseBy * Level);
}
