using Godot;

[GlobalClass]
public partial class GrassHoneyGainBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.05f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Grass", "noun_grass")} tiles boost {Style.CK("flowers'", "noun_flower")} honey by {Style.NCPercent(GameStore.GrassHoneyGainBuff.Value, GameStore.GrassHoneyGainBuff.Value + IncreaseBy, !IsMaxLevel())}";

    public override void Apply() => GameStore.GrassHoneyGainBuff.AddFlat(Name, IncreaseBy * Level);
}
