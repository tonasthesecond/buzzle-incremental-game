using Godot;

[GlobalClass]
public partial class CloverPollinationTimeUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    public override string GetHoverDescription() =>
        $"{Style.CK("Clover Pollination Time", "noun_clover")} {Style.NumberChange(GameStore.CloverPollinationTime.Value, GameStore.CloverPollinationTime.Value + IncreaseBy)}s";

    public override void Apply()
    {
        GameStore.CloverPollinationTime.AddFlat(Name, IncreaseBy * Level);
    }
}
