using Godot;

[GlobalClass]
public partial class LoamYarrowHoneyGainBuffUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 0.1f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Loam", "noun_loam")} boost {Style.CK("Yarrows", "noun_yarrow")}' honey production by {Style.NumberChange(GameStore.LoamYarrowHoneyGainBuff.Value, GameStore.LoamYarrowHoneyGainBuff.Value + IncreaseBy)}";

    public override void Apply() =>
        GameStore.LoamYarrowHoneyGainBuff.AddFlat(Name, IncreaseBy * Level);
}
