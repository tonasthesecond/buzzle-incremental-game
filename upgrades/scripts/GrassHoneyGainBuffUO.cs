using Godot;

[GlobalClass]
public partial class GrassHoneyGainBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.05f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Grass", "noun_grass")} boost {Style.CK("flowers'", "noun_flower")} honey production by {Style.NumberChange(GameStore.GrassHoneyGainBuff.Value * 100f, (GameStore.GrassHoneyGainBuff.Value + IncreaseBy) * 100f)}"
        + "%";

    public override void Apply() => GameStore.GrassHoneyGainBuff.AddFlat(Name, IncreaseBy * Level);
}
