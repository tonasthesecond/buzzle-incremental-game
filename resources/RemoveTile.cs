using Godot;

[GlobalClass]
public partial class RemoveTile : SelectedResource
{
    public override string GetHoverTitle() => "Remove Tile";

    public override string GetHoverDescription() => "Remove a tile from the board.";

    public override string GetHoverSubtitle() => "";

    public override int GetHoverCost() => 0;

    public override bool IsEnough() => true;
}
