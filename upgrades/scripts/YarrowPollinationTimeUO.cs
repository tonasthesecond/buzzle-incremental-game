using Godot;

[GlobalClass]
public partial class YarrowPollinationTimeUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Yarrows", "noun_yarrow")} pollinate in {Style.NC(GameStore.YarrowPollinationTime.Value, GameStore.YarrowPollinationTime.Value + IncreaseBy, showChange: !IsMaxLevel())}s";

    public override void Apply() => GameStore.YarrowPollinationTime.AddFlat(Name, IncreaseBy * Level);
}
