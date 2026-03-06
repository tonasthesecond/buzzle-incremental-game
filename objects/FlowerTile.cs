using Godot;

[GlobalClass]
public partial class FlowerTile : BaseTile
{
    public FlowerType type;

    bool isPollinated = false;

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
                isPollinated = false;
                Modulate = Colors.DarkGray;
            }
        }
    }

    public void Init(FlowerType type)
    {
        this.type = type;
        Modulate = Colors.DarkGray;
    }

    public void Pollinate(int honeyAddition)
    {
        Honey += honeyAddition;
        if (Honey >= type.HoneyCost)
        {
            Modulate = Colors.White;
            isPollinated = true;
            Honey = type.HoneyAmount;
        }
    }
}
