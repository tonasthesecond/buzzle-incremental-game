using Godot;

[GlobalClass]
public partial class SunflowerPollinationTimeUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Sunflowers", "noun_sunflower")} pollinate in {Style.NC(GameStore.SunflowerPollinationTime.Value, GameStore.SunflowerPollinationTime.Value + IncreaseBy, showChange: !IsMaxLevel())}s";

    public override void Apply() => GameStore.SunflowerPollinationTime.AddFlat(Name, IncreaseBy * Level);
}
