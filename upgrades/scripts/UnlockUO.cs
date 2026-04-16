using Godot;

[GlobalClass]
public partial class UnlockUO : IUpgradeOption
{
    [Export]
    public string UnlockKey { get; set; } = "";

    [Export]
    public string Description { get; set; } = "";

    public override string GetHoverDescription() =>
        $"Unlock {Style.CK(Name, "noun_" + UnlockKey.ToLower())}. {Description}";

    public override int MaxLevel { get; set; } = 1;

    public override void Apply()
    {
        GameStore.Unlock(UnlockKey);
    }
}
