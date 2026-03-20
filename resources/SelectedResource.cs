using Godot;

/// A container for a resource and its image.
[GlobalClass]
public partial class SelectedResource : Resource
{
    [Export]
    public Resource Resource;

    [Export]
    public Texture2D Icon;
}
