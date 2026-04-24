using Godot;

[GlobalClass]
public partial class FatBeeCapacityHoneySetInfiniteUO : IUpgradeOption
{
    public override string GetTechnicalText() =>
        $"{Style.CK("Fat bees", "noun_fat")} carry {Style.NC((int)GameStore.FatBeeCapacityHoneyBonus.Value, 9999, !IsMaxLevel())} honey (effectively unlimited)";

    public override void Apply()
    {
        if (Level <= 0)
            return;
        GameStore.FatBeeCapacityHoneyBonus.AddFlat(Name, 5000);
        GameStore.FatBeeCapacityHoneyInfinite = true;
    }
}
