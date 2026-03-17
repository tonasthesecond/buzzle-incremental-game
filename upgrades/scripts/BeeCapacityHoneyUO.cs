using Godot;

[GlobalClass]
public partial class BeeCapacityHoneyUO : UpgradeOption
{
    public override string GetText() =>
        $"Bees can carry {GameStore.BeeCapacityHoney} ➞ {GameStore.BeeCapacityHoney + 1} honey.";

    public override int GetCost() => 20;

    private int x0 = 1;

    public override void Apply() => GameStore.BeeCapacityHoney = x0 + Level;
}
