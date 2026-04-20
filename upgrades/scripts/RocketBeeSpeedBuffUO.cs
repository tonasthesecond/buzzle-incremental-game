using Godot;

[GlobalClass]
public partial class RocketBeeSpeedBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.5f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Rocket bee", "noun_rocket")} during charged flight will move +{Style.NCPercent(GameStore.RocketBeeSpeedBuff.Value, GameStore.RocketBeeSpeedBuff.Value + IncreaseBy)} faster";

    public override void Apply() => GameStore.RocketBeeSpeedBuff.AddFlat(Name, IncreaseBy * Level);
}
