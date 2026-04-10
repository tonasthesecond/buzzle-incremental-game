using Godot;

[GlobalClass]
public partial class FatBeeSpeedDebuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.05f;

    [Export]
    public int BaseCost { get; set; } = 10;

    public override string GetText() =>
        $"{Style.CK("Fat Bee Speed Penalty", "noun_fat_bee")} {Style.NumberChange(GameStore.FatBeeSpeedDebuff.Value, GameStore.FatBeeSpeedDebuff.Value + IncreaseBy)}x";

    public override int GetCost() => Level * BaseCost;

    public override void Apply()
    {
        GameStore.FatBeeSpeedDebuff.AddFlat(Name, IncreaseBy * Level);
    }
}
