using Godot;

[GlobalClass]
public partial class RocketBeeChargeTimeUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -100f;

    public override string GetHoverDescription() => 
        $"{Style.CK("Rocket Bee Charge Time", "noun_rocket_bee")} {Style.NumberChange(GameStore.RocketBeeChargeTime.Value, GameStore.RocketBeeChargeTime.Value + IncreaseBy)}ms";

    public override void Apply()
    {
        GameStore.RocketBeeChargeTime.AddFlat(Name, IncreaseBy * Level);
    }
}
