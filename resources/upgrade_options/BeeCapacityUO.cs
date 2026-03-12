using Godot;

[GlobalClass]
public partial class BeeCapacityUO : UpgradeOption
{
    public override string GetText() =>
        $"bees can carry {GameStore.BeeCapacity} ➞ {GameStore.BeeCapacity + 1} honey";

    public override int GetCost() => 50;

    public override void Apply() => GameStore.BeeCapacity += 1;
}
