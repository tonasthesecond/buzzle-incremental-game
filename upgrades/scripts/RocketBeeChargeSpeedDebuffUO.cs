using Godot;

[GlobalClass]
public partial class RocketBeeChargeSpeedDebuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.05f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Jetpack bee", "noun_rocket")} charge speed penalty {Style.NCPercent(GameStore.RocketBeeChargeSpeedDebuff.Value, GameStore.RocketBeeChargeSpeedDebuff.Value + IncreaseBy, !IsMaxLevel())}";

    public override void Apply() => GameStore.RocketBeeChargeSpeedDebuff.AddFlat(Name, IncreaseBy * Level);
}
