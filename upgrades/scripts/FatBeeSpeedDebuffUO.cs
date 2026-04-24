using Godot;

[GlobalClass]
public partial class FatBeeSpeedDebuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.05f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Fat bee", "noun_fat")} speed penalty {Style.NCPercent(GameStore.FatBeeSpeedDebuff.Value, GameStore.FatBeeSpeedDebuff.Value + IncreaseBy, !IsMaxLevel())}";

    public override void Apply() => GameStore.FatBeeSpeedDebuff.AddFlat(Name, IncreaseBy * Level);
}
