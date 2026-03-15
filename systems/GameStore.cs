using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;

public partial class GameStore : Node
{
    public static GameStore Instance { get; private set; } = null!;

    [Signal]
    public delegate void OnHoneyChangedEventHandler(int newHoney);

    // --- Computed Stats ---
    public static int HiveCapacityBee { get; set; } = 10;
    public static float BeeSpeed { get; set; } = 50;
    public static int BeeCapacityHoney { get; set; } = 1;
    public static int BeeCount => Save.Hives.Sum(h => h.BeeCount);

    // --- Honey ---
    public static int Honey
    {
        get => Save.Honey;
        set
        {
            if (Save.Honey == value)
                return;
            Save.Honey = value;
            Instance.EmitSignal(SignalName.OnHoneyChanged, value);
        }
    }

    // --- Save Data ---
    public static SaveData Save { get; private set; } = new();

    private const string SavePath = "user://save.json";
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    public static void SaveGame()
    {
        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        file.StoreString(JsonSerializer.Serialize(Save, JsonOpts));
    }

    // --- Upgrades ---
    public static void ApplyUpgrades()
    {
        foreach (var saved in Save.Upgrades)
        {
            var upgrade = GD.Load<UpgradeOption>(saved.Path);
            if (upgrade == null)
            {
                GD.PushError($"[GameStore] Missing upgrade: {saved.Path}");
                continue;
            }
            upgrade.Level = saved.Level;
            for (int i = 0; i < saved.Level; i++)
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
        ApplyUpgrades();
        Services.Get<BeeSystem>().SpawnFromSave();
    }
}
