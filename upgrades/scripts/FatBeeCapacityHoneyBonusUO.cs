using Godot;

[GlobalClass]
public partial class FatBeeCapacityHoneyBonusUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    [Export]
    public int BaseCost { get; set; } = 10;

    public override string GetText() =>
        $"{Style.CK("Fat Bee Carry Bonus", "noun_fat_bee")} {Style.NumberChange(GameStore.FatBeeCapacityHoneyBonus.Value, GameStore.FatBeeCapacityHoneyBonus.Value + IncreaseBy)}";

    public override int GetCost() => Level * BaseCost;

    public override void Apply()
    {
        GameStore.FatBeeCapacityHoneyBonus.AddFlat(Name, IncreaseBy * Level);
    }
}
