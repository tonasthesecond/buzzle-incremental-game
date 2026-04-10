using Godot;

[GlobalClass]
public partial class RosePerTileFromHiveHoneyGainBonusUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.5f;

    public override string GetHoverDescription() => 
        $"{Style.CK("Rose Hive Distance Bonus", "noun_rose")} {Style.NumberChange(GameStore.RosePerTileFromHiveHoneyGainBonus.Value, GameStore.RosePerTileFromHiveHoneyGainBonus.Value + IncreaseBy)} per tile";

    public override void Apply()
    {
        GameStore.RosePerTileFromHiveHoneyGainBonus.AddFlat(Name, IncreaseBy * Level);
    }
}
