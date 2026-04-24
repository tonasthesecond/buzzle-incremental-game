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

        return $"{Style.CK("Hives", "noun_hive")} gain +{IncreaseBy} capacity per unlocked {Style.CK("bee", "noun_bee")} caste\n"
             + $"Total capacity {Style.NC(currentHiveCapacity, newHiveCapacity, !IsMaxLevel())} ({currentBeeCasteCount} castes unlocked)";
    }

    public override void Apply() => GameStore.HiveCapacityBee.AddFlat(Name, Level * IncreaseBy * currentBeeCasteCount);
}
