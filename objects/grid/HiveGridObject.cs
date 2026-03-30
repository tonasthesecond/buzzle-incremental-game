using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class HiveGridObject : BaseGridObject
{
    [Signal]
    public delegate void BeeAddedEventHandler(BeeEntity bee);

    private List<BeeEntity> bees = new();

    public int BeeCount => bees.Count;

    public void AddBee(BeeEntity bee)
    {
        bees.Add(bee);
        bee.Home = this;
        EmitSignal(SignalName.BeeAdded, bee);
    }

    /// Type name -> count for save data.
    public Dictionary<string, int> GetBeeCounts()
    {
        var counts = new Dictionary<string, int>();
        foreach (var bee in bees)
        {
            var type = bee.GetType().Name;
            counts.TryGetValue(type, out int cur);
            counts[type] = cur + 1;
        }
        return counts;
    }

    /// Deposits the given amount of honey into the hive.
    public void Deposit(int amount)
    {
        GameStore.Honey += amount;
    }

    /// Takes up to amount honey from the hive's reserve.
    public int TakePossible(int amount)
    {
        int possible = int.Min(amount, GameStore.Honey);
        GameStore.Honey -= possible;
        return possible;
    }
}
