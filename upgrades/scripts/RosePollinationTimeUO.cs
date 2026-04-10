using Godot;

[GlobalClass]
public partial class RosePollinationTimeUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    [Export]
    public int BaseCost { get; set; } = 10;

    public override string GetText() =>
        $"{Style.CK("Rose Pollination Time", "noun_rose")} {Style.NumberChange(GameStore.RosePollinationTime.Value, GameStore.RosePollinationTime.Value + IncreaseBy)}s";

    public override int GetCost() => Level * BaseCost;

    public override void Apply()
    {
        GameStore.RosePollinationTime.AddFlat(Name, IncreaseBy * Level);
    }
}
