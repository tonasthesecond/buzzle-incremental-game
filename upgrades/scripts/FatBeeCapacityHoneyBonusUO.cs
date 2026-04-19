using Godot;

[GlobalClass]
public partial class FatBeeCapacityHoneyBonusUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    public override string GetTechnicalText() => 
        $"{Style.CK("Fat Bee Carry Bonus", "noun_fat_bee")} {Style.NumberChange(GameStore.FatBeeCapacityHoneyBonus.Value, GameStore.FatBeeCapacityHoneyBonus.Value + IncreaseBy)}";

    public override void Apply()
    {
        GameStore.FatBeeCapacityHoneyBonus.AddFlat(Name, IncreaseBy * Level);
    }
}
