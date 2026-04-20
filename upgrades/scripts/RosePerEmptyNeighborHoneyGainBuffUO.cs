using Godot;

[GlobalClass]
public partial class RosePerEmptyNeighborHoneyGainBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.05f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Roses", "noun_rose")} produce +{Style.NCPercent(GameStore.RosePerEmptyNeighborHoneyGainBuff.Value, GameStore.RosePerEmptyNeighborHoneyGainBuff.Value + IncreaseBy)} more honey per empty neighbor tile";

    public override void Apply() =>
        GameStore.RosePerEmptyNeighborHoneyGainBuff.AddFlat(Name, IncreaseBy * Level);
}
