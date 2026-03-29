using Godot;

[GlobalClass]
public partial class HiveCapacityBeeUO : IUpgradeOption
{
    public override string GetText() =>
        $"Hives can store {GameStore.HiveCapacityBee.Value} -> {GameStore.HiveCapacityBee.Value + Level} bees.";

    public override int GetCost() => Level * 10;

    public override void Apply()
    {
        GameStore.HiveCapacityBee.AddFlat(Name, Level);
    }
}
