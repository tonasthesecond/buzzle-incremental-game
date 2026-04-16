using Godot;

[GlobalClass]
public partial class RemoveBee : SelectedResource
{
    [Export]
    public PackedScene BeeScene { get; set; }

    public override string GetHoverTitle() => "Remove Bee";

    public override string GetHoverDescription() => "Remove a bee from a hive.";

    public override string GetHoverSubtitle() => "";

    public override int GetHoverCost() => 0;

    public override bool IsEnough() => true;
}
