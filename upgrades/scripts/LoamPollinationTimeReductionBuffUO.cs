using Godot;

[GlobalClass]
public partial class LoamPollinationTimeReductionBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.05f;

    public override string GetHoverDescription() => 
        $"{Style.CK("Loam Pollination Speed Buff", "noun_loam")} {Style.NumberChange(GameStore.LoamPollinationTimeReductionBuff.Value * 100f, (GameStore.LoamPollinationTimeReductionBuff.Value + IncreaseBy) * 100f)}"
        + "%";

    public override void Apply()
    {
        GameStore.LoamPollinationTimeReductionBuff.AddFlat(Name, IncreaseBy * Level);
    }
}
