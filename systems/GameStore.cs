using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;

public partial class GameStore : Node
{
    public static GameStore Instance { get; private set; } = null!;

    [Signal]
    public delegate void HoneyChangedEventHandler(int newHoney);

    [Signal]
    public delegate void OnUnlockedEventHandler(string key);

    // --- Computed Stats ---
    public static Stat HiveCapacityBee { get; } = new(5f);
    public static Stat HiveCapacityBeePerBeeCasteBonus { get; } = new(0f);

    public static Stat BeeSpeed { get; } = new(30f);
    public static Stat BeeCapacityHoney { get; } = new(1f);

    public static Stat BeekeeperEffectZoneRadius { get; } = new(32f);
    public static Stat BeekeeperEffectZoneSpeedBuff { get; } = new(0.5f);
    public static Stat BeekeeperEffectZoneFadeoutTime { get; } = new(1f);
    public static bool BeekeeperEffectZoneNeverFade { get; set; } = false;

    public static Stat QueenBeeEffectZoneRadius { get; } = new(32f);
    public static Stat QueenBeeEffectZoneSpeedBuff { get; } = new(0.5f);
    public static Stat QueenBeeEffectZonePollinationTimeReductionBuff { get; } = new(0.2f);
    public static Stat QueenBeeBeePriceReductionBuff { get; } = new(0f);
    public const float QueenBeeBeePriceReductionBuffMax = 0.99f;
    public static bool QueenBeeLeashRose { get; set; } = false;

    public static Stat FatBeeSpeedDebuff { get; } = new(0.7f);
    public static Stat FatBeeCapacityHoneyBonus { get; } = new(5f);
    public static Stat FatBeeSpeedPerRocketBeeBuff { get; } = new(0f);
    public static bool FatBeeCapacityHoneyInfinite { get; set; } = false;

    public static Stat RocketBeeSpeedBuff { get; } = new(2f);
    public static Stat RocketBeeChargeSpeedDebuff { get; } = new(0.2f);
    public static Stat RocketBeeChargeTime { get; } = new(2000f);
    public static Stat RocketBeeChargeDistance { get; } = new(20f);
    public static Stat RocketBeeCapacityHoneyPerFatBeeBonus { get; } = new(0f);
    public static bool RocketBeeIsolatedHarvest { get; set; } = false;

    public static Stat PoppyHoneyCost { get; } = new(1f);
    public static Stat PoppyHoneyGain { get; } = new(2f);
    public static Stat PoppyPollinationTime { get; } = new(3f);

    public static Stat SunflowerHoneyCost { get; } = new(1f);
    public static Stat SunflowerHoneyGain { get; } = new(2f);
    public static Stat SunflowerPollinationTime { get; } = new(6f);
    public static Stat SunflowerHoneyGainPerFatBeeBonus { get; } = new(0f);

    public static Stat CloverHoneyCost { get; } = new(3f);
    public static Stat CloverRegularHoneyGain { get; } = new(3f);
    public static Stat CloverJackpotHoneyGain { get; set; } = new(7f);
    public static Stat CloverPollinationTime { get; } = new(5f);
    public static Stat CloverJackpotChance { get; set; } = new(0.1f);

    public static Stat YarrowHoneyCost { get; } = new(4f);
    public static Stat YarrowHoneyGain { get; } = new(6f);
    public static Stat YarrowPollinationTime { get; } = new(7f);
    public static Stat YarrowPerSameNeighborHoneyGainBuff { get; } = new(0.05f);

    public static Stat RoseHoneyCost { get; } = new(5f);
    public static Stat RoseHoneyGain { get; } = new(1f);
    public static Stat RosePollinationTime { get; } = new(15f);
    public static Stat RosePerTileFromHiveHoneyGainBonus { get; } = new(1f);
    public static Stat RosePerEmptyNeighborHoneyGainBuff { get; } = new(0.1f);

    public static Stat DirtPoppyHoneyGainBuff { get; } = new(0f);

    public static Stat GrassHoneyGainBuff { get; } = new(0.2f);
    public static Stat GrassCloverJackpotChanceBonus { get; } = new(0f);

    public static Stat LoamPollinationTimeReductionBuff { get; } = new(0.2f);
    public static Stat LoamYarrowHoneyGainBuff { get; } = new(0f);

    // --- Placement Price Models ---
    public static Dictionary<Type, IScaleModel> PriceModels { get; } =
        new()
        {
            {
                typeof(Poppy),
                new PaddingModel(new float[] { 0f, 0f }, new PolynomialModel(5f, 0.7f))
            },
            { typeof(Sunflower), new PolynomialModel(20f, 0.7f) },
            { typeof(Clover), new PolynomialModel(25f, 0.4f) },
            { typeof(Yarrow), new PolynomialModel(15f, 0.2f) },
            { typeof(Rose), new PolynomialModel(50f, 0.2f) },
            { typeof(Hive), new PaddingModel(new float[] { 0f }, new ExponentialModel(10f, 1.5f)) },
            {
                typeof(DirtTile),
                new PaddingModel(new float[] { 0f, 0f }, new PolynomialModel(5f, 0.5f))
            },
            { typeof(LoamTile), new PolynomialModel(10f, 0.6f) },
            { typeof(GrassTile), new PolynomialModel(10f, 0.7f) },
            {
                typeof(BaseBee),
                new PaddingModel(new float[] { 0f, 0f }, new PolynomialModel(10f, 0.5f))
            },
            { typeof(FatBee), new PolynomialModel(10f, 0.6f) },
            { typeof(RocketBee), new PolynomialModel(10f, 0.6f) },
            { typeof(QueenBee), new PolynomialModel(50f, 0.6f) },
        };

    public static int GetPlacementCost(Type t)
    {
        if (!PriceModels.TryGetValue(t, out var model))
            return 0;
        Grid grid = Services.Get<Grid>()!;
        int count =
            t.IsSubclassOf(typeof(BaseTile)) ? grid.GetTileCountOfType(t)
            : t.IsSubclassOf(typeof(BaseGridObject)) ? grid.GetObjectCountOfType(t)
            : Services.Get<BeeSystem>().GetBeeCountOfType(t);
        if (t.IsSubclassOf(typeof(Bee)))
            return (int)(
                model.Get(count)
                * (
                    1
                    - Mathf.Min(
                        GameStore.QueenBeeBeePriceReductionBuffMax,
                        GameStore.QueenBeeBeePriceReductionBuff.Value
                    )
                )
            ); // queen bee price reduction
        return (int)(model.Get(count));
    }

    // --- Unlocks ---
    public static readonly string[] AllUnlocks =
    [
        "Sunflower",
        "Clover",
        "Yarrow",
        "Rose",
        "Grass",
        "Loam",
        "Queen",
        "Rocket",
        "Fat",
    ];
    private static readonly HashSet<string> unlockedKeys = new HashSet<string>([
        "Poppy",
        "Dirt",
        "Bee",
    ]);

    public static void Unlock(string key)
    {
        if (unlockedKeys.Add(key))
        {
            Save.UnlockedKeys.Add(key);
            Instance.EmitSignal(SignalName.OnUnlocked, key);
        }
    }

    public static void UnlockAll()
    {
        foreach (var key in AllUnlocks)
            Unlock(key);
    }

    public static string[] GetUnlockedBeeKeys()
    {
        string[] beeKeys = ["Queen", "Rocket", "Fat", "Bee"];
        return beeKeys.Where(key => IsUnlocked(key)).ToArray();
    }

    public static string[] GetUnlockedTileKeys()
    {
        string[] tileKeys = ["Grass", "Loam"];
        return tileKeys.Where(key => IsUnlocked(key)).ToArray();
    }

    public static string[] GetUnlockedFlowerKeys()
    {
        string[] flowerKeys = ["Sunflower", "Clover", "Yarrow", "Rose", "Poppy"];
        return flowerKeys.Where(key => IsUnlocked(key)).ToArray();
    }

    public static bool IsUnlocked(string key) => unlockedKeys.Contains(key);

    // --- Honey ---
    private static int honey { get; set; } = 10;
    public static int Honey
    {
        get => honey;
        set
        {
            honey = Mathf.Max(1, value);
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

    public static void LoadGame()
    {
        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);

        // load save data
        Save = FileAccess.FileExists(SavePath)
            ? JsonSerializer.Deserialize<SaveData>(FileAccess.GetFileAsString(SavePath), JsonOpts)!
            : new SaveData();

        if (Save == null)
            Save = new SaveData();

        // apply all dynamic contents
        Honey = Save.Honey;
        Services.Get<UpgradeTree>().ApplyUpgrades();

        Callable
            .From(() =>
            {
                SignalBus.Instance.EmitSignal(SignalBus.SignalName.GameLoaded);
            })
            .CallDeferred();
    }

    public static void SaveGame()
    {
        Save.Honey = Honey;
        Services.Get<Grid>().Snapshot();
        Services.Get<UpgradeTree>().SaveUpgrades();
        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        file.StoreString(JsonSerializer.Serialize(Save, JsonOpts));
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
    }
}
