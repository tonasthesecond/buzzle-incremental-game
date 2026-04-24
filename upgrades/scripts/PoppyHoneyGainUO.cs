using Godot;

[GlobalClass]
public partial class PoppyHoneyGainUO : IUpgradeOption
{
    [Export]
    public int IncreaseBy { get; set; } = 1;

    public override string GetTechnicalText() =>
        $"{Style.CK("Poppies", "noun_poppy")} yield {Style.NC((int)GameStore.PoppyHoneyGain.Value, (int)(GameStore.PoppyHoneyGain.Value + IncreaseBy), !IsMaxLevel())} honey";

    public override void Apply() => GameStore.PoppyHoneyGain.AddFlat(Name, IncreaseBy * Level);
}
