using Godot;

[GlobalClass]
public partial class BlackholeNegativePullSpeedUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 10f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Blackhole", "noun_blackhole")} makes {Style.CK("bees", "noun_bee")} moving away have -({Style.NC(Utils.PixelsToTiles(GameStore.BlackholeNegativePullSpeed.Value), Utils.PixelsToTiles(GameStore.BlackholeNegativePullSpeed.Value + IncreaseBy), showChange: !IsMaxLevel())}) tiles/sec speed";

    public override void Apply() =>
        GameStore.BlackholeNegativePullSpeed.AddFlat(Name, IncreaseBy * Level);
}
