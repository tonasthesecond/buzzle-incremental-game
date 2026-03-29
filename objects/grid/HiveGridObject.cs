using Godot;

[GlobalClass]
public partial class HiveGridObject : BaseGridObject
{
    [Signal]
    public delegate void BeeAddedEventHandler(Bee bee);

    public int BeeCount { get; set; } = 0;

    public void AddBee(Bee bee)
    {
        BeeCount++;
        bee.Home = this;
        EmitSignal(SignalName.BeeAdded, bee);
    }

    /// Deposits the given amount of honey into the hive, returning the leftover amount that couldn't be deposited.
    public void Deposit(int amount)
    {
        GameStore.Honey += amount;
    }

    /// Takes the given amount of honey from the hive, clamping it to the hive's reserve.
    public int TakePossible(int amount)
    {
        int possibleAmount = int.Min(amount, GameStore.Honey);
        GameStore.Honey -= possibleAmount;
        return possibleAmount;
    }
}
