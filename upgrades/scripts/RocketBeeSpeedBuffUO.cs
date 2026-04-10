using Godot;

[GlobalClass]
public partial class RocketBeeSpeedBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.5f;

    public override string GetHoverDescription() => 
        $"{Style.CK("Rocket Bee Speed Buff", "noun_rocket_bee")} {Style.NumberChange(GameStore.RocketBeeSpeedBuff.Value, GameStore.RocketBeeSpeedBuff.Value + IncreaseBy)}x";

    public override void Apply()
    {
        GameStore.RocketBeeSpeedBuff.AddFlat(Name, IncreaseBy * Level);
    }
}
