using Godot;

[GlobalClass]
public partial class FatBeeCapacityHoneySetInfiniteUO : IUpgradeOption
{
    public override string GetTechnicalText() =>
        $"{Style.CK("Fat bees", "noun_fat")} can carry {Style.NC(GameStore.FatBeeCapacityHoneyBonus.Value.ToString("F0"), "inf")} honey";

    public override void Apply()
    {
        GameStore.FatBeeCapacityHoneyBonus.AddFlat(Name, 5000);
        GameStore.FatBeeCapacityHoneyInfinite = true;
    }
}
