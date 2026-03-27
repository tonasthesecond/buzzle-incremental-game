using Godot;

[GlobalClass]
public partial class BaseFlower : BaseGridObject
{
    public bool IsPollinated = false;
    public int HoneyCost { get; set; } = 1;
    public int HoneyAmount { get; set; } = 2;

    private int honey = 0;
    public int Honey
    {
        get => honey;
        set
        {
            honey = value;

            // If honey is depleted, the flower is no longer pollinated.
            if (honey <= 0)
            {
                IsPollinated = false;
                Modulate = Colors.DarkGray;
            }
        }
    }

    public void Pollinate(int honeyAddition)
    {
        Honey += honeyAddition;
        if (Honey >= HoneyCost)
        {
            Modulate = Colors.White;
            IsPollinated = true;
            Honey = HoneyAmount;
        }
    }

    public override void _Ready()
    {
        base._Ready();
        IsPollinated = false;
        Modulate = Colors.DarkGray;
    }
}
