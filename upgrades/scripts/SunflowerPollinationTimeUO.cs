using Godot;

[GlobalClass]
public partial class SunflowerPollinationTimeUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    public override string GetTechnicalText() => 
        $"{Style.CK("Sunflower Pollination Time", "noun_sunflower")} {Style.NumberChange(GameStore.SunflowerPollinationTime.Value, GameStore.SunflowerPollinationTime.Value + IncreaseBy)}s";

    public override void Apply()
    {
        GameStore.SunflowerPollinationTime.AddFlat(Name, IncreaseBy * Level);
    }
}
