using Godot;

[GlobalClass]
public partial class RosePollinationTimeUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Roses", "noun_rose")} pollinate in {Style.NC(GameStore.RosePollinationTime.Value, GameStore.RosePollinationTime.Value + IncreaseBy, showChange: !IsMaxLevel())}s";

    public override void Apply() => GameStore.RosePollinationTime.AddFlat(Name, IncreaseBy * Level);
}
