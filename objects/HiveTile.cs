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
}
