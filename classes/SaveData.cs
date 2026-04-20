using System.Collections.Generic;

public class SaveData
{
    public int Honey { get; set; } = 11;
    public List<SavedUpgrade> Upgrades { get; set; } = new();
    public List<SavedHive> Hives { get; set; } = new();
    public List<SavedTileGroup> Tiles { get; set; } = new();
    public List<SavedObject> Objects { get; set; } = new();
    public List<string> UnlockedKeys { get; set; } = new();
}

public class SavedUpgrade
{
    public string Id { get; set; } = "";
    public int Level { get; set; }
}

public class SavedHive
{
    public int X { get; set; }
    public int Y { get; set; }

    public Dictionary<string, int> BeeCounts { get; set; } = new();
}

public class SavedTileGroup
{
    public string Type { get; set; } = "";
    public List<int[]> Spans { get; set; } = new();
}

public class SavedObject
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Type { get; set; } = "";
}
