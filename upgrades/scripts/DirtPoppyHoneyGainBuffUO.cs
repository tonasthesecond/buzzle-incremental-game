using Godot;

[GlobalClass]
public partial class DirtPoppyHoneyGainBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 1f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Dirt", "noun_dirt")} boost {Style.CK("Poppies", "noun_poppy")}' honey production by {Style.NumberChange(GameStore.DirtPoppyHoneyGainBuff.Value, GameStore.DirtPoppyHoneyGainBuff.Value + IncreaseBy)}";

    public override void Apply()
    {
        GameStore.DirtPoppyHoneyGainBuff.AddFlat(Name, IncreaseBy * Level);
    }
}
