using Godot;

[GlobalClass]
public partial class BeeCapacityHoneyUO : IUpgradeOption
{
    [Export]
    public int IncreaseBy { get; set; } = 1;

    public override string GetTechnicalText() =>
        $"{Style.CK("Bees", "noun_bee")} can carry {Style.NC((int)GameStore.BeeCapacityHoney.Value, (int)(GameStore.BeeCapacityHoney.Value + IncreaseBy), !IsMaxLevel())} honey";

    public override void Apply() => GameStore.BeeCapacityHoney.AddFlat(Name, IncreaseBy * Level);
}
