using Godot;

/// A container for a resource and its image.
[GlobalClass]
public partial class SelectedResource : Resource
{
    [Export]
    public Resource Resource = new Resource();

    [Export]
    public string ImagePath = "res://assets/bee.png";
}
