using Godot;

[GlobalClass]
public partial class BlackholePullRangeUO : IUpgradeOption
{
    [Export]
    public float IncreaseBy { get; set; } = 32f; // 1 tile

    public override string GetTechnicalText() =>
        $"{Style.CK("Blackholes", "noun_blackhole")} affect {Style.CK("bees", "noun_bee")} up to {Style.NC(Utils.PixelsToTiles(GameStore.BlackholePullRange.Value), Utils.PixelsToTiles(GameStore.BlackholePullRange.Value + IncreaseBy), 0, showChange: !IsMaxLevel())} tiles away";

    public override void Apply() => GameStore.BlackholePullRange.AddFlat(Name, IncreaseBy * Level);
}
