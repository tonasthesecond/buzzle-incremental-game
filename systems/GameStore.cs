using System.Collections.Generic;
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

    public static Stat BeekeeperEffectZoneRadius { get; } = new(32f);
    public static Stat BeekeeperEffectZoneSpeedBuff { get; } = new(0.5f);
    public static Stat BeekeeperEffectZoneFadeoutTime { get; } = new(1f);

    public static Stat QueenBeeEffectZoneRadius { get; } = new(32f);
    public static Stat QueenBeeEffectZoneSpeedBuff { get; } = new(0.5f);

    public static Stat FatBeeSpeedDebuff { get; } = new(0.4f);
    public static Stat FatBeeCapacityHoneyBonus { get; } = new(5f);

    public static Stat RocketBeeSpeedBuff { get; } = new(2f);
    public static Stat RocketBeeChargeSpeedDebuff { get; } = new(0.2f);
    public static Stat RocketBeeChargeTime { get; } = new(2000f);
    public static Stat RocketBeeChargeDistance { get; } = new(20f);

    public static Stat BaseFlowerHoneyCost { get; } = new(1f);
    public static Stat BaseFlowerHoneyGain { get; } = new(2f);
    public static Stat BaseFlowerPollinationTime { get; } = new(3f);

    public static Stat CloverHoneyCost { get; } = new(2f);
    public static Stat CloverRegularHoneyGain { get; } = new(2f);
    public static Stat CloverJackpotHoneyGain { get; } = new(7f);
    public static Stat CloverJackpotChance { get; } = new(0.1f);
    public static Stat CloverPollinationTime { get; } = new(3f);

    public static Stat SmoothSoilPollinationTimeReductionBuff { get; } = new(0.2f);

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
        Services.Get<Grid>().Snapshot();
        SaveUpgrades(Services.Get<UpgradeTree>());
        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        file.StoreString(JsonSerializer.Serialize(Save, JsonOpts));
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
    public const int ValidTileDistance = 3;

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

        Callable
            .From(() =>
            {
                SignalBus.Instance.EmitSignal(SignalBus.SignalName.GameLoaded);
            })
            .CallDeferred();
    }
}
