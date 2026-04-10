using Godot;

[GlobalClass]
public partial class RocketBeeChargeSpeedDebuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.05f;

    [Export]
    public int BaseCost { get; set; } = 10;

    public override string GetText() =>
        $"{Style.CK("Rocket Bee Charge Speed", "noun_rocket_bee")} {Style.NumberChange(GameStore.RocketBeeChargeSpeedDebuff.Value, GameStore.RocketBeeChargeSpeedDebuff.Value + IncreaseBy)}x";

    public override int GetCost() => Level * BaseCost;

    public override void Apply()
    {
        GameStore.RocketBeeChargeSpeedDebuff.AddFlat(Name, IncreaseBy * Level);
    }
}
