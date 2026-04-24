using Godot;

[GlobalClass]
public partial class RocketBeeSpeedBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.5f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Jetpack bees", "noun_rocket")} fly {Style.NCPercent(GameStore.RocketBeeSpeedBuff.Value, GameStore.RocketBeeSpeedBuff.Value + IncreaseBy, !IsMaxLevel())} faster during launch";

    public override void Apply() => GameStore.RocketBeeSpeedBuff.AddFlat(Name, IncreaseBy * Level);
}
