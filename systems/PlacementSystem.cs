using Godot;

[GlobalClass]
public partial class PlacementSystem : GameSystem
{
    private PackedScene? selectedScene;

    public override void _Ready()
    {
        SignalBus.Instance.ResourceSelected += (resource) =>
        {
            if (resource is PackedScene scene)
                selectedScene = scene;
        };

        SignalBus.Instance.ResourceUnselected += () => selectedScene = null;
    }

    /// Handle tile placement on click.
    public override void _UnhandledInput(InputEvent e)
    {
        if (selectedScene == null)
            return;
        if (e is not InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true } mouse)
            return;

        var grid = Services.Get<Grid>();
        var tilemap = Services.Get<Tilemap>();
        var pos = tilemap.LocalToMap(tilemap.GetLocalMousePosition());

        if (!grid.PlaceTile(selectedScene, pos, out var fail))
            GD.Print($"[PlacementSystem] {fail}");
    }
}
