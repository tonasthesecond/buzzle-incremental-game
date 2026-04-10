using Godot;

[GlobalClass]
public partial class DirtTile : BaseTile, IHasHoverTitle, IHasHoverDescription
{
    public override string GetHoverTitle() => "Dirt Tile";

    public override string GetHoverDescription() => "Boring dirt tile. You can put stuff on it?";
}
