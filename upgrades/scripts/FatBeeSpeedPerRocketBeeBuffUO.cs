using Godot;

[GlobalClass]
public partial class FatBeeSpeedPerRocketBeeBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.05f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Fat bees", "noun_fat")} move +{Style.NCPercent(GameStore.FatBeeSpeedPerRocketBeeBuff.Value, GameStore.FatBeeSpeedPerRocketBeeBuff.Value + IncreaseBy)}";

    public override void Apply() =>
        GameStore.FatBeeSpeedPerRocketBeeBuff.AddFlat(Name, IncreaseBy * Level);
}
