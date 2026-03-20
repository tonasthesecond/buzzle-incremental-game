using Godot;

[GlobalClass]
public partial class BeeCountDisplay : NumberDisplay
{
    public override void _Ready()
    {
        base._Ready();
        BeeSystem? beeSystem = Services.Get<BeeSystem>();
        if (beeSystem == null)
        {
            GD.PrintErr("[BeeCountDisplay] BeeSystem not found");
            return;
        }
        beeSystem.OnBeeSpawned += (_) => SetNumber(beeSystem.GetBeeCount());
        SetNumber(beeSystem.GetBeeCount());
    }
}
