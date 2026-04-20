using Godot;

[GlobalClass]
public partial class RemoveBee : SelectedResource
{
    [Export]
    public string BeeTypeName { get; set; } = "";

    private Bee? bee;

    public override string GetHoverTitle() =>
        $"Remove {Style.CK(BeeTypeName == "Rocket" ? "Jetpack" : BeeTypeName, "noun_" + BeeTypeName.ToLower())} bee";

    public override string GetHoverDescription() =>
        $"Remove a {Style.CK(BeeTypeName == "Rocket" ? "Jetpack" : BeeTypeName, "noun_" + BeeTypeName.ToLower())} bee from a hive.";

    public override string GetHoverSubtitle() => "";

    public override int GetHoverCost() => 0;

    public override bool IsEnough() => true;
}
