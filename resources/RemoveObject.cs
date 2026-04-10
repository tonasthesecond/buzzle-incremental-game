using Godot;

[GlobalClass]
public partial class RemoveObject : SelectedResource
{
    public override string GetHoverTitle() => "Remove Object";

    public override string GetHoverDescription() => "Remove an object from the board.";

    public override string GetHoverSubtitle() => "";

    public override int GetHoverCost() => 0;

    public override bool IsEnough() => true;
}
