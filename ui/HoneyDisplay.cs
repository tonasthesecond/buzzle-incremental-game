using Godot;

[GlobalClass]
public partial class HoneyDisplay : NumberDisplay
{
    public override void _Ready()
    {
        base._Ready();
        GameStore.Instance.HoneyChanged += SetNumber;
        SetNumber(GameStore.Honey);
    }

    public override void _ExitTree()
    {
        if (GameStore.Instance != null)
            GameStore.Instance.HoneyChanged -= SetNumber;
    }
}
