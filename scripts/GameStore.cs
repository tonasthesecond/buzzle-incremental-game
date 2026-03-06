using System.Collections.Generic;
using System.Linq;
using Godot;

public class GameStore
{
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

    public static FlowerType GetRandomFlower()
    {
        var keys = FlowerTypes.Keys.ToList();
        string key = keys[(int)GD.Randi() % keys.Count];
        return FlowerTypes[key];
    }
}
