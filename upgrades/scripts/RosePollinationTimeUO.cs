using Godot;

[GlobalClass]
public partial class RosePollinationTimeUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    public override string GetTechnicalText() => 
        $"{Style.CK("Rose Pollination Time", "noun_rose")} {Style.NumberChange(GameStore.RosePollinationTime.Value, GameStore.RosePollinationTime.Value + IncreaseBy)}s";

    public override void Apply()
    {
        GameStore.RosePollinationTime.AddFlat(Name, IncreaseBy * Level);
    }
}
