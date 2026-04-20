using Godot;

[GlobalClass]
public partial class HiveCapacityBeePerBeeCasteBonusUO : IUpgradeOption
{
    [Export]
    public int IncreaseBy { get; set; } = 1;

    private int currentBeeCasteCount => GameStore.GetUnlockedBeeKeys().Length;

    public override string GetTechnicalText()
    {
        int currentHiveCapacity = (int)GameStore.HiveCapacityBee.Value;
        int? currentBoost = (int?)GameStore.HiveCapacityBee.Get(Name);
        int newHiveCapacity = currentHiveCapacity + (Level * IncreaseBy) * currentBeeCasteCount;
        if (currentBoost != null)
            newHiveCapacity -= (int)currentBoost;

        return $"{Style.CK("Hives", "noun_hive")} can hold an additional {Style.NC(IncreaseBy, newHiveCapacity, 0)} {Style.CK("bees", "noun_bee")} per {Style.CK("bee", "noun_bee")} caste unlocked ({Style.NC(currentHiveCapacity, newHiveCapacity)})";
    }

    public override void Apply() =>
        GameStore.HiveCapacityBee.AddFlat(Name, Level * IncreaseBy * currentBeeCasteCount);
}
