using System.Collections.Generic;
using System.Linq;
using Godot;

/// Tracks honey income with per-source attribution.
/// Call Record() at deposit sites; query with windowed or all-time methods.
public partial class HoneyTracker : GameSystem
{
    private record Sample(ulong TimeMs, string Source, int Amount);

    private readonly List<Sample> samples = new();
    private readonly Dictionary<string, int> allTime = new();

    public static HoneyTracker Instance { get; private set; } = null!;

    public override void _Ready()
    {
        Services.Register(this);
        Instance = this;
    }

    /// Record a honey deposit from a named source.
    public void Record(string source, int amount)
    {
        samples.Add(new Sample(Time.GetTicksMsec(), source, amount));
        allTime[source] = allTime.GetValueOrDefault(source) + amount;
    }

    // --- Windowed queries ---

    private IEnumerable<Sample> InWindow(ulong windowMs)
    {
        ulong cutoff = Time.GetTicksMsec() - windowMs;
        return samples.Where(s => s.TimeMs >= cutoff);
    }

    private void Prune(ulong keepMs = 300000) // drop samples older than 5 min by default
    {
        ulong cutoff = Time.GetTicksMsec() - keepMs;
        samples.RemoveAll(s => s.TimeMs < cutoff);
    }

    /// Honey per second over the given window.
    public float GetHPS(ulong windowMs = 5000)
    {
        Prune();
        return InWindow(windowMs).Sum(s => s.Amount) / (windowMs / 1000f);
    }

    /// HPS broken down by source, over the given window.
    public Dictionary<string, float> GetHPSBySource(ulong windowMs = 5000)
    {
        Prune();
        return InWindow(windowMs)
            .GroupBy(s => s.Source)
            .ToDictionary(g => g.Key, g => g.Sum(s => s.Amount) / (windowMs / 1000f));
    }

    /// Top k sources by HPS over the given window.
    public List<(string Source, float HPS)> GetTopSources(int k, ulong windowMs = 5000)
    {
        return GetHPSBySource(windowMs)
            .OrderByDescending(kv => kv.Value)
            .Take(k)
            .Select(kv => (kv.Key, kv.Value))
            .ToList();
    }

    // --- All-time queries ---

    /// Total honey ever recorded, optionally filtered to one source.
    public int GetAllTime(string? source = null)
    {
        if (source != null)
            return allTime.GetValueOrDefault(source);
        return allTime.Values.Sum();
    }

    /// All-time totals by source, sorted descending.
    public List<(string Source, int Total)> GetAllTimeBySource(int k = int.MaxValue)
    {
        return allTime
            .OrderByDescending(kv => kv.Value)
            .Take(k)
            .Select(kv => (kv.Key, kv.Value))
            .ToList();
    }
}
