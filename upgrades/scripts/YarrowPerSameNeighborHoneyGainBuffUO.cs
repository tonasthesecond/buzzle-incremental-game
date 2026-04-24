using Godot;

[GlobalClass]
public partial class YarrowPerSameNeighborHoneyGainBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.05f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Yarrows", "noun_yarrow")} gain +{Style.NCPercent(GameStore.YarrowPerSameNeighborHoneyGainBuff.Value, GameStore.YarrowPerSameNeighborHoneyGainBuff.Value + IncreaseBy, !IsMaxLevel())} honey per adjacent {Style.CK("Yarrow", "noun_yarrow")}";

    public override void Apply() => GameStore.YarrowPerSameNeighborHoneyGainBuff.AddFlat(Name, IncreaseBy * Level);
}
