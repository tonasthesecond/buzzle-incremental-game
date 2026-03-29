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
}
