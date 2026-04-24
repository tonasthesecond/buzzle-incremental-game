using Godot;

[GlobalClass]
public partial class FatBeeSpeedBuffPerRocketBeeBonusUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.05f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Fat bees", "noun_fat")} gain +{Style.NCPercent(GameStore.FatBeeSpeedPerRocketBeeBuff.Value, GameStore.FatBeeSpeedPerRocketBeeBuff.Value + IncreaseBy, !IsMaxLevel())} speed per {Style.CK("Jetpack bee", "noun_rocket")}";

    public override void Apply() => GameStore.FatBeeSpeedPerRocketBeeBuff.AddFlat(Name, IncreaseBy * Level);
}
