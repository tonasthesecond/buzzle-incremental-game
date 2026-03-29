using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;

public partial class GameStore : Node
{
    public static GameStore Instance { get; private set; } = null!;

    [Signal]
    public delegate void HoneyChangedEventHandler(int newHoney);

    // --- Computed Stats ---
    public static Stat HiveCapacityBee { get; } = new(10f);
    public static Stat BeeSpeed { get; } = new(50f);
    public static Stat BeeCapacityHoney { get; } = new(1f);

    public static Stat BeekeeperRadius { get; } = new(32f);
    public static Stat BeekeeperSpeedBuff { get; } = new(0.5f);

    // --- Honey ---
    private static int honey { get; set; } = 10;
    public static int Honey
    {
        get => honey;
        set
        {
            honey = value;
            Save.Honey = honey;
            Instance.EmitSignal(SignalName.HoneyChanged, value);
        }
    }

    // --- Save Data ---
    public static SaveData Save { get; private set; } = new();

    private const string SavePath = "res://user/save.json";
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    public static void SaveGame()
    {
        Save.Honey = Honey;
        SaveGrid(Services.Get<Grid>());
        SaveUpgrades(Services.Get<UpgradeTree>());
        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        file.StoreString(JsonSerializer.Serialize(Save, JsonOpts));
    }

    // --- Tiles ---
    public static void SaveGrid(Grid grid)
    {
        GameStore.Save.Tiles = grid.GetTiles()
            .Select(t => new SavedTile
            {
                X = t.GridPosition.X,
                Y = t.GridPosition.Y,
                Type = t.GetType().Name,
            })
            .ToList();

        GameStore.Save.Objects = grid.GetObjects()
            .Select(o => new SavedObject
            {
                X = o.GridPosition.X,
                Y = o.GridPosition.Y,
                Type = o.GetType().Name,
            })
            .ToList();

        GameStore.Save.Hives = grid.GetObjectsOfType<HiveGridObject>()
            .Select(h => new SavedHive
            {
                X = h.GridPosition.X,
                Y = h.GridPosition.Y,
                BeeCount = h.BeeCount,
            })
            .ToList();
    }

    // --- Upgrades ---
    public static void SaveUpgrades(UpgradeTree tree)
    {
        GameStore.Save.Upgrades.Clear();
        foreach (var upgrade in tree.GetUpgrades())
        {
            var saved = new SavedUpgrade
            {
                Id = upgrade.ResourcePath.GetFile().GetBaseName(),
                Level = upgrade.Level,
            };
            GameStore.Save.Upgrades.Add(saved);
        }
    }

    public static void ApplyUpgrades()
    {
        foreach (SavedUpgrade saved in Save.Upgrades)
        {
            var upgrade = GD.Load<IUpgradeOption>("res://upgrades/resources/" + saved.Id + ".tres");
            if (upgrade == null)
            {
                GD.PushError($"[GameStore] Missing upgrade: {saved.Id}");
                continue;
            }
            upgrade.Level = saved.Level;
            for (int i = 0; i <= saved.Level; i++)
                upgrade.Apply();
        }
    }

    // --- Constants ---
    public const int TILE_SIZE = 32;

    public static readonly Dictionary<string, string> Colors = JsonSerializer.Deserialize<
        Dictionary<string, string>
    >(FileAccess.GetFileAsString("res://resources/colors.json"))!;

    // --- Ready ---
    public override void _Ready()
    {
        Instance = this;

        // load save data
        Save = FileAccess.FileExists(SavePath)
            ? JsonSerializer.Deserialize<SaveData>(FileAccess.GetFileAsString(SavePath), JsonOpts)!
            : new SaveData();

        // apply all dynamic contents
        honey = Save.Honey;
        ApplyUpgrades();
        Callable.From(Services.Get<BeeSystem>().SpawnFromSave).CallDeferred();
    }
}
