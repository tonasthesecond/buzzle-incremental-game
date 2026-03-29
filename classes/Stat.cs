using System;
using System.Collections.Generic;
using System.Linq;

public class Stat
{
    private readonly Func<float> baseValue;
    private readonly Dictionary<string, float> flat = new();
    private readonly Dictionary<string, float> percent = new();

    public Stat(float baseValue)
        : this(() => baseValue) { }

    public Stat(Func<float> baseValue) => this.baseValue = baseValue; // live base

    public float Value => (baseValue() + flat.Values.Sum()) * (1f + percent.Values.Sum());

    public void AddFlat(string key, float value) => flat[key] = value;

    public void AddPercent(string key, float value) => percent[key] = value;

    public void Remove(string key)
    {
        flat.Remove(key);
        percent.Remove(key);
    }
}
