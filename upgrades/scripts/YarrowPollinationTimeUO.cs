using Godot;

[GlobalClass]
public partial class YarrowPollinationTimeUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    public override string GetTechnicalText() => 
        $"{Style.CK("Yarrow Pollination Time", "noun_yarrow")} {Style.NumberChange(GameStore.YarrowPollinationTime.Value, GameStore.YarrowPollinationTime.Value + IncreaseBy)}s";

    public override void Apply()
    {
        GameStore.YarrowPollinationTime.AddFlat(Name, IncreaseBy * Level);
    }
}
