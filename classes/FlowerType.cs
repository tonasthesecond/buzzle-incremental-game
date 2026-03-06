public class FlowerType
{
    public int HoneyCost { get; set; } = 1;
    public int HoneyAmount { get; set; } = 2;
    public string ImagePath { get; private set; }

    public FlowerType(int honeyCost, int honeyAmount, string imagePath)
    {
        HoneyCost = honeyCost;
        HoneyAmount = honeyAmount;
        ImagePath = imagePath;
    }
}
