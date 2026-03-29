using Godot;

[GlobalClass]
public partial class BeeSpeedUO : UpgradeOption
{
    [Export]
    public int IncreaseBy { get; set; } = 10;

    public override string GetText() => $"bee speed lvl{Level}";

    public override int GetCost() => (Level + 1) * 10;

    public override void Apply()
    {
        GameStore.BeeSpeed.AddFlat(Name, IncreaseBy * Level);
    }
}
