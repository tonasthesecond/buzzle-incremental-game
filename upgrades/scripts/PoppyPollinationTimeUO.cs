using Godot;

[GlobalClass]
public partial class PoppyPollinationTimeUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    [Export]
    public int BaseCost { get; set; } = 10;

    public override string GetText() =>
        $"{Style.CK("Poppy Pollination Time", "noun_poppy")} {Style.NumberChange(GameStore.PoppyPollinationTime.Value, GameStore.PoppyPollinationTime.Value + IncreaseBy)}s";

    public override int GetCost() => Level * BaseCost;

    public override void Apply()
    {
        GameStore.PoppyPollinationTime.AddFlat(Name, IncreaseBy * Level);
    }
}
