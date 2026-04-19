using Godot;

[GlobalClass]
public partial class HiveCapacityBeeUO : IUpgradeOption
{
    public override string GetTechnicalText() => 
        $"Hives can store {GameStore.HiveCapacityBee.Value} -> {GameStore.HiveCapacityBee.Value + Level} bees.";

    public override void Apply()
    {
        GameStore.HiveCapacityBee.AddFlat(Name, Level);
    }
}
