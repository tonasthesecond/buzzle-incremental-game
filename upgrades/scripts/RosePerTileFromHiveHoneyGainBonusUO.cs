using Godot;

[GlobalClass]
public partial class RosePerTileFromHiveHoneyGainBonusUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.5f;

    [Export]
    public int BaseCost { get; set; } = 10;

    public override string GetText() =>
        $"{Style.CK("Rose Hive Distance Bonus", "noun_rose")} {Style.NumberChange(GameStore.RosePerTileFromHiveHoneyGainBonus.Value, GameStore.RosePerTileFromHiveHoneyGainBonus.Value + IncreaseBy)} per tile";

    public override int GetCost() => Level * BaseCost;

    public override void Apply()
    {
        GameStore.RosePerTileFromHiveHoneyGainBonus.AddFlat(Name, IncreaseBy * Level);
    }
}
