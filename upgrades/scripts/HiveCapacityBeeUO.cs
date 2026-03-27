using Godot;

[GlobalClass]
public partial class HiveCapacityBeeUO : UpgradeOption
{
    public override string GetText() =>
        $"Hives can store {GameStore.HiveCapacityBee} -> {GameStore.HiveCapacityBee} bees.";

    public override int GetCost() => Level * 10;

    private int x0 = 10;

    public override void Apply()
    {
        GameStore.HiveCapacityBee = x0 + Level;
    }
}
