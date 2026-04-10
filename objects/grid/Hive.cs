using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class Hive : BaseGridObject
{
    [Signal]
    public delegate void BeeAddedEventHandler(Bee bee);

    private List<Bee> bees = new();

    public int BeeCount => bees.Count;

    public void AddBee(Bee bee)
    {
        bees.Add(bee);
        bee.Home = this;
        EmitSignal(SignalName.BeeAdded, bee);
    }

    /// Type name -> count for save data.
    public Dictionary<string, int> GetBeeCounts()
    {
        var counts = new Dictionary<string, int>();
        foreach (Bee bee in bees)
        {
            string type = bee.GetType().Name;
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

    public override string GetHoverDescription()
    {
        Dictionary<string, int> beeCounts = GetBeeCounts();
        string desc = $"A home for {Style.CK(GameStore.HiveCapacityBee.Value.ToString())} bees.\n";
        Dictionary<string, int> beeTypesCounts = new();
        foreach (Bee bee in bees)
        {
            string type = bee.BeeTypeName;
            beeTypesCounts.TryGetValue(type, out int cur);
            beeTypesCounts[type] = cur + 1;
        }
        foreach (KeyValuePair<string, int> beeType in beeTypesCounts)
        {
            string noun = beeType.Key.ToLower();
            if (noun == "")
                desc += $"\n{beeType.Value} bees";
            else
                desc += $"\n{beeType.Value} {Style.CK(beeType.Key, "noun_" + noun)} bees";
        }
        return desc;
    }
}
