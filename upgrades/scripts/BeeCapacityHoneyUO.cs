using Godot;

[GlobalClass]
public partial class BeeCapacityHoneyUO : IUpgradeOption
{
    [Export]
    public int IncreaseBy { get; set; } = 1;

    public override string GetTechnicalText() =>
        $"Bees can carry {GameStore.BeeCapacityHoney.Value} ➞ {GameStore.BeeCapacityHoney.Value + IncreaseBy} honey.";

    public override void Apply()
    {
        GameStore.BeeCapacityHoney.AddFlat(Name, IncreaseBy * Level);
    }
}
