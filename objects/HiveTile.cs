using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class HiveTile : BaseTile
{
    public List<Bee> Bees { get; private set; } = new();
    public int Honey { get; set; } = 1;

    public int Harvesters => Bees.FindAll(b => b.job is HarvesterJob).Count;
    public int Pollinators => Bees.FindAll(b => b.job is PollinatorJob).Count;

    public void AddBee(Bee bee)
    {
        if (Bees.Count >= GameStore.HiveBeeCapacity)
            return;
        Bees.Add(bee);
    }

    public void RemoveBee(Bee bee)
    {
        Bees.Remove(bee);
    }

    /// Deposits the given amount of honey into the hive, returning the leftover amount that couldn't be deposited.
    public int DepositMax(int amount)
    {
        int possibleAmount = int.Min(amount, GameStore.HiveHoneyCapacity - Honey);
        Honey += possibleAmount;
        GameStore.Honey += possibleAmount;
        return amount - possibleAmount;
    }

    /// Takes the given amount of honey from the hive, clamping it to the hive's reserve.
    public int TakePossible(int amount)
    {
        int possibleAmount = int.Min(amount, Honey);
        Honey -= possibleAmount;
        GameStore.Honey -= possibleAmount;
        return possibleAmount;
    }
}
