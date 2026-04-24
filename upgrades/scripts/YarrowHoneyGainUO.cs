using Godot;

[GlobalClass]
public partial class YarrowHoneyGainUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Yarrows", "noun_yarrow")} yield {Style.NC(GameStore.YarrowHoneyGain.Value, GameStore.YarrowHoneyGain.Value + IncreaseBy, showChange: !IsMaxLevel())} honey";

    public override void Apply() => GameStore.YarrowHoneyGain.AddFlat(Name, IncreaseBy * Level);
}
