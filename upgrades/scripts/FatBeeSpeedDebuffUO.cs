using Godot;

[GlobalClass]
public partial class FatBeeSpeedDebuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.05f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Fat Bee Speed Penalty", "noun_fat_bee")} {Style.NumberChange(GameStore.FatBeeSpeedDebuff.Value, GameStore.FatBeeSpeedDebuff.Value + IncreaseBy)}x";

    public override void Apply()
    {
        GameStore.FatBeeSpeedDebuff.AddFlat(Name, IncreaseBy * Level);
    }
}
