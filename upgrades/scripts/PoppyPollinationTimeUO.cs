using Godot;

[GlobalClass]
public partial class PoppyPollinationTimeUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    public override string GetHoverDescription() => 
        $"{Style.CK("Poppy Pollination Time", "noun_poppy")} {Style.NumberChange(GameStore.PoppyPollinationTime.Value, GameStore.PoppyPollinationTime.Value + IncreaseBy)}s";

    public override void Apply()
    {
        GameStore.PoppyPollinationTime.AddFlat(Name, IncreaseBy * Level);
    }
}
