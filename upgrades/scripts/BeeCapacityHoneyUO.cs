using Godot;

[GlobalClass]
public partial class BeeCapacityHoneyUO : IUpgradeOption
{
    public override string GetText() =>
        $"Bees can carry {GameStore.BeeCapacityHoney.Value} ➞ {GameStore.BeeCapacityHoney.Value + 1} honey.";

    public override int GetCost() => 20;

    public override void Apply() => GameStore.BeeCapacityHoney.AddFlat(Name, Level);
}
