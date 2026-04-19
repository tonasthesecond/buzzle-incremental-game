using Godot;

[GlobalClass]
public partial class UnlockUO : IUpgradeOption
{
    [Export]
    public string UnlockKey { get; set; } = "";

    public override int MaxLevel { get; set; } = 1;

    public override string GetTechnicalText() => $"";

    public override void Apply()
    {
        GameStore.Unlock(UnlockKey);
    }
}
