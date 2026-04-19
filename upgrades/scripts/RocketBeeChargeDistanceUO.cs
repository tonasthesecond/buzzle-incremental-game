using Godot;

[GlobalClass]
public partial class RocketBeeChargeDistanceUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 5f;

    public override string GetTechnicalText() => 
        $"{Style.CK("Rocket Bee Charge Distance", "noun_rocket_bee")} {Style.NumberChange(GameStore.RocketBeeChargeDistance.Value, GameStore.RocketBeeChargeDistance.Value + IncreaseBy)} tiles";

    public override void Apply()
    {
        GameStore.RocketBeeChargeDistance.AddFlat(Name, IncreaseBy * Level);
    }
}
