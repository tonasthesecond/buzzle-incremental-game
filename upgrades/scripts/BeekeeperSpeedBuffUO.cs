using Godot;

[GlobalClass]
public partial class BeekeeperSpeedBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.1f;

    private float value => GameStore.BeekeeperSpeedBuff.Value;

    public override string GetText() =>
        $"{Style.CK("Beekeeper Effect Zone", "noun_beekeeper")} increases all {Style.CK("Bees", "noun_bee")}' speed by {Style.NumberChangePercent(value, value + IncreaseBy)}";

    public override void Apply()
    {
        GameStore.BeekeeperSpeedBuff.AddFlat(Name, IncreaseBy * Level);
    }
}
