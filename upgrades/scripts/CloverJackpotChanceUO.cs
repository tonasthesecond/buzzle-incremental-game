using Godot;

[GlobalClass]
public partial class CloverJackpotChanceUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.05f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Clover Jackpot Chance", "noun_clover")} {Style.NumberChange(GameStore.CloverJackpotChance.Value * 100f, (GameStore.CloverJackpotChance.Value + IncreaseBy) * 100f)}"
        + "%";

    public override void Apply()
    {
        GameStore.CloverJackpotChance.AddFlat(Name, IncreaseBy * Level);
    }
}
