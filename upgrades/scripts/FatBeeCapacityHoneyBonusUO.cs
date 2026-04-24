using Godot;

[GlobalClass]
public partial class FatBeeCapacityHoneyBonusUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Fat bees", "noun_fat")} carry {Style.NC(GameStore.FatBeeCapacityHoneyBonus.Value, GameStore.FatBeeCapacityHoneyBonus.Value + IncreaseBy, showChange: !IsMaxLevel())} bonus honey";

    public override void Apply() => GameStore.FatBeeCapacityHoneyBonus.AddFlat(Name, IncreaseBy * Level);
}
