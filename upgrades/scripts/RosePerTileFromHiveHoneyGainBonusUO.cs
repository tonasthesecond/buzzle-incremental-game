using Godot;

[GlobalClass]
public partial class RosePerTileFromHiveHoneyGainBonusUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.5f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Roses", "noun_rose")} gain +{Style.NC(GameStore.RosePerTileFromHiveHoneyGainBonus.Value, GameStore.RosePerTileFromHiveHoneyGainBonus.Value + IncreaseBy, showChange: !IsMaxLevel())} honey per tile from hive";

    public override void Apply() => GameStore.RosePerTileFromHiveHoneyGainBonus.AddFlat(Name, IncreaseBy * Level);
}
