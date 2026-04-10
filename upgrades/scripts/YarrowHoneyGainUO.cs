using Godot;

[GlobalClass]
public partial class YarrowHoneyGainUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    public override string GetHoverDescription() => 
        $"{Style.CK("Yarrow Honey Yield", "noun_yarrow")} {Style.NumberChange(GameStore.YarrowHoneyGain.Value, GameStore.YarrowHoneyGain.Value + IncreaseBy)}";

    public override void Apply()
    {
        GameStore.YarrowHoneyGain.AddFlat(Name, IncreaseBy * Level);
    }
}
