using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class Hive : BaseGridObject, IHasHoverRefresh, IHasHoverSubtitle
{
    [Signal]
    public delegate void BeeAddedEventHandler(Bee bee);

    [Signal]
    public delegate void BeeRemovedEventHandler(Bee bee);

    public List<Bee> Bees = new();

    public int BeeCount => Bees.Count;

    public void AddBee(Bee bee)
    {
        Bees.Add(bee);
        bee.Home = this;
        EmitSignal(SignalName.BeeAdded, bee);
    }

    public void RemoveBee(Bee bee)
    {
        Bees.Remove(bee);
        EmitSignal(SignalName.BeeRemoved, bee);
    }

    /// Type name -> count for save data.
    public Dictionary<string, int> GetBeeCounts()
    {
        var counts = new Dictionary<string, int>();
        foreach (Bee bee in Bees)
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
        string desc = $"A home for {Style.CK(GameStore.HiveCapacityBee.Value.ToString())} bees.\n";
        if (GetBeeCounts().Count == 0)
            desc += "\nEmpty";
        else
        {
            desc += "\nHousing:";
            foreach (KeyValuePair<string, int> beeType in GetBeeCounts())
            {
                string noun = beeType.Key.ToLower().Substr(0, beeType.Key.Length - 3);
                if (noun == "jetpack")
                    noun = "rocket";
                if (noun == "base")
                    desc += $"\n• {Style.CK(beeType.Value.ToString("F0"))} bees";
                else
                    desc +=
                        $"\n• {Style.CK(beeType.Value.ToString("F0"))} {Style.CK(beeType.Key, "noun_" + noun)} bees";
            }
        }
        return desc;
    }

    public string GetHoverSubtitle()
    {
        if (!Placed)
            return "";
        int beeCount = GetBeeCounts().Values.Sum();
        string text = "bees";
        if (beeCount == 1)
            text = "bee";
        return $"{beeCount}/{GameStore.HiveCapacityBee.Value} {text}";
    }

    private BeeSystem.BeeSpawnedEventHandler? beeSpawnedHandler;
    private BeeRemovedEventHandler? beeRemovedHandler;

    public void RegisterRefresh(Action onRefresh)
    {
        beeSpawnedHandler = (_) => onRefresh();
        beeRemovedHandler = (_) => onRefresh();
        Services.Get<BeeSystem>().BeeSpawned += beeSpawnedHandler;
        BeeRemoved += beeRemovedHandler;
    }

    public void UnregisterRefresh(Action onRefresh)
    {
        if (beeSpawnedHandler != null)
        {
            Services.Get<BeeSystem>().BeeSpawned -= beeSpawnedHandler;
            beeSpawnedHandler = null;
        }
        if (beeRemovedHandler != null)
        {
            BeeRemoved -= beeRemovedHandler;
            beeRemovedHandler = null;
        }
    }
}
