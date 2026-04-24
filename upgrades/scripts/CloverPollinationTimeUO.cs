using Godot;

[GlobalClass]
public partial class CloverPollinationTimeUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Clovers", "noun_clover")} pollinate in {Style.NC(GameStore.CloverPollinationTime.Value, GameStore.CloverPollinationTime.Value + IncreaseBy, showChange: !IsMaxLevel())}s";

    public override void Apply() => GameStore.CloverPollinationTime.AddFlat(Name, IncreaseBy * Level);
}
