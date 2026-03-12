using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;

public partial class GameStore : Node
{
    public static GameStore Instance { get; private set; }

    [Signal]
    public delegate void OnHoneyChangedEventHandler(int newHoney);
    public static int Honey
    {
        get => honey;
        set
        {
            if (honey == value)
                return;
            honey = value;
            Instance.EmitSignal(SignalName.OnHoneyChanged, honey);
        }
    }
    private static int honey = 11;

    public static int BeeCount = 2;
    public static float BeeSpeed = 50;
    public static int BeeCapacity = 1;

    public static int HiveBeeCapacity = 10;
    public static int HiveHoneyCapacity = 10;

    public const int TILE_SIZE = 32;
    public static readonly Dictionary<string, FlowerType> FlowerTypes = new()
    {
        { "Red", new FlowerType(1, 2, "res://flowers/RedFlower.png") },
        { "Blue", new FlowerType(2, 4, "res://flowers/BlueFlower.png") },
        { "Gold", new FlowerType(5, 10, "res://flowers/GoldFlower.png") },
    };
    public static readonly FlowerType DefaultFlowerType = FlowerTypes["Red"];

    public static readonly Dictionary<string, string> Colors = JsonSerializer.Deserialize<
        Dictionary<string, string>
    >(FileAccess.GetFileAsString("res://resources/colors.json"));

    public static FlowerType GetRandomFlowerType()
    {
        var keys = FlowerTypes.Keys.ToList();
        string key = keys[(int)GD.Randi() % keys.Count];
        return FlowerTypes[key];
    }

    public override void _Ready()
    {
        Instance = this;
    }
}
