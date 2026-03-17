using System.Collections.Generic;

public class SaveData
{
    public int Honey { get; set; } = 10;
    public List<SavedUpgrade> Upgrades { get; set; } = new();
    public List<SavedHive> Hives { get; set; } = new();
    public List<SavedTile> Tiles { get; set; } = new();
    public List<SavedObject> Objects { get; set; } = new();
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
    public int BeeCount { get; set; }
}

public class SavedTile
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Type { get; set; } = "";
}

public class SavedObject
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Type { get; set; } = "";
}
