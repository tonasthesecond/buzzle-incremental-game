using Godot;

[GlobalClass]
public partial class CloverJackpotChanceUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.05f;

    [Export]
    public int BaseCost { get; set; } = 10;

    public override string GetText() =>
        $"{Style.CK("Clover Jackpot Chance", "noun_clover")} {Style.NumberChange(GameStore.CloverJackpotChance.Value * 100f, (GameStore.CloverJackpotChance.Value + IncreaseBy) * 100f)}"
        + "%";

    public override int GetCost() => Level * BaseCost;

    public override void Apply()
    {
        GameStore.CloverJackpotChance.AddFlat(Name, IncreaseBy * Level);
    }
}
