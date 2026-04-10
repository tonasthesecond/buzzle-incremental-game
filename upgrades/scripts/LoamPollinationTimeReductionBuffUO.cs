using Godot;

[GlobalClass]
public partial class LoamPollinationTimeReductionBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.05f;

    [Export]
    public int BaseCost { get; set; } = 10;

    public override string GetText() =>
        $"{Style.CK("Loam Pollination Speed Buff", "noun_loam")} {Style.NumberChange(GameStore.LoamPollinationTimeReductionBuff.Value * 100f, (GameStore.LoamPollinationTimeReductionBuff.Value + IncreaseBy) * 100f)}"
        + "%";

    public override int GetCost() => Level * BaseCost;

    public override void Apply()
    {
        GameStore.LoamPollinationTimeReductionBuff.AddFlat(Name, IncreaseBy * Level);
    }
}
