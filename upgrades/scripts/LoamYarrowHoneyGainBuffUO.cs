using Godot;

[GlobalClass]
public partial class LoamYarrowHoneyGainBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.1f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Loam", "noun_loam")} tiles boost {Style.CK("Yarrows'", "noun_yarrow")} honey by {Style.NCPercent(GameStore.LoamYarrowHoneyGainBuff.Value, GameStore.LoamYarrowHoneyGainBuff.Value + IncreaseBy, !IsMaxLevel())}";

    public override void Apply() => GameStore.LoamYarrowHoneyGainBuff.AddFlat(Name, IncreaseBy * Level);
}
