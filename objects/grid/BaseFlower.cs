using Godot;

[GlobalClass]
public partial class BaseFlower : BaseGridObject
{
    public bool IsPollinated = false;
    public int HoneyCost { get; set; } = 1;
    public int HoneyAmount { get; set; } = 2;

    private int honey = 0; // honey in the flower, tracks pollination progress and output as well
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

    /// Pollinate the flower, adding honey.
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

    /// How many honey is required to pollinate this flower.
    public int HoneyRequired()
    {
        if (!IsPollinated)
            return HoneyCost - Honey;
        return 0;
    }

    public override void _Ready()
    {
        base._Ready();
        IsPollinated = false;
        Modulate = Colors.DarkGray;
    }
}
