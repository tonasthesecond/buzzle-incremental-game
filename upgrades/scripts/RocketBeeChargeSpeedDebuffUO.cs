using Godot;

[GlobalClass]
public partial class RocketBeeChargeSpeedDebuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.05f;

    public override string GetHoverDescription() => 
        $"{Style.CK("Rocket Bee Charge Speed", "noun_rocket_bee")} {Style.NumberChange(GameStore.RocketBeeChargeSpeedDebuff.Value, GameStore.RocketBeeChargeSpeedDebuff.Value + IncreaseBy)}x";

    public override void Apply()
    {
        GameStore.RocketBeeChargeSpeedDebuff.AddFlat(Name, IncreaseBy * Level);
    }
}
