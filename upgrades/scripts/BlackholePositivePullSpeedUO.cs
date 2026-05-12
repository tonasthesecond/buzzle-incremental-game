using Godot;

[GlobalClass]
public partial class BlackholePositivePullSpeedUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 10f;

    public override string GetTechnicalText() =>
        $"{Style.CK("Blackhole", "noun_blackhole")} makes {Style.CK("bees", "noun_bee")} moving toward have +({Style.NC(Utils.PixelsToTiles(GameStore.BlackholePositivePullSpeed.Value), Utils.PixelsToTiles(GameStore.BlackholePositivePullSpeed.Value + IncreaseBy), showChange: !IsMaxLevel())}) tiles/sec speed";

    public override void Apply() =>
        GameStore.BlackholePositivePullSpeed.AddFlat(Name, IncreaseBy * Level);
}
