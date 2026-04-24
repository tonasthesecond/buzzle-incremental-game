using Godot;

[GlobalClass]
public partial class PoppyPollinationTimeUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = -0.5f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Poppies", "noun_poppy")} pollinate in {Style.NC(GameStore.PoppyPollinationTime.Value, GameStore.PoppyPollinationTime.Value + IncreaseBy, showChange: !IsMaxLevel())}s";

    public override void Apply() => GameStore.PoppyPollinationTime.AddFlat(Name, IncreaseBy * Level);
}
