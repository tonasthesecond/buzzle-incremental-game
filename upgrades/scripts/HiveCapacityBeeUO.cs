using Godot;

[GlobalClass]
public partial class HiveCapacityBeeUO : IUpgradeOption
{
    [Export]
    public int IncreaseBy { get; set; } = 1;

    public override string GetTechnicalText() =>
        $"{Style.CK("Hives", "noun_hive")} can hold {Style.NumberChange(GameStore.HiveCapacityBee.Value, GameStore.HiveCapacityBee.Value + IncreaseBy)} {Style.CK("bees", "noun_bee")}";

    public override void Apply() => GameStore.HiveCapacityBee.AddFlat(Name, Level);
}
