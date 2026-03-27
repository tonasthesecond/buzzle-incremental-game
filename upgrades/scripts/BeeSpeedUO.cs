using Godot;

[GlobalClass]
public partial class BeeSpeedUO : UpgradeOption
{
    public override string GetText() => $"bee speed lvl{Level}";

    public override int GetCost() => Level * 10;

    private int x0 = 50;

    public override void Apply()
    {
        GameStore.BeeSpeed = (x0 + Level * 10);
    }
}
