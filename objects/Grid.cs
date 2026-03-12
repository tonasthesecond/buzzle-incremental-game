using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class Grid : Node2D
{
    public int Width { get; set; } = 11;
    public int Height { get; set; } = 11;

    private Dictionary<Vector2I, BaseTile> occupied_tiles = new();
    private static List<Vector2I> empty_tiles = new();
    private static Dictionary<Type, PackedScene> tile_scenes = new();

    public override void _Ready()
    {
        // register tile service
        Services.Register(this);

        // register tile types
        RegisterTile<HiveTile>(GD.Load<PackedScene>("res://objects/HiveTile.tscn"));
        RegisterTile<FlowerTile>(GD.Load<PackedScene>("res://objects/FlowerTile.tscn"));

        // all tiles are empty
        for (int x = 0; x < Width; x++)
        for (int y = 0; y < Height; y++)
            empty_tiles.Add(new Vector2I(x, y));

        // TEST: place tiles
        Vector2I[] flower_tiles =
        [
            new Vector2I(0, 0),
            new Vector2I(1, 0),
            new Vector2I(9, 9),
            new Vector2I(3, 4),
            new Vector2I(7, 3),
        ];
        foreach (var tile in flower_tiles)
        {
            var flower = PlaceTile<FlowerTile>(tile);
            flower.Init(GameStore.DefaultFlowerType);
        }
        PlaceTile<HiveTile>(new Vector2I(5, 5));
    }

    public override void _Draw()
    {
        // draw grid lines
        for (int x = 0; x <= Width; x++)
            DrawLine(
                new Vector2(x * GameStore.TILE_SIZE, 0),
                new Vector2(x * GameStore.TILE_SIZE, Height * GameStore.TILE_SIZE),
                Colors.White,
                1f
            );

        for (int y = 0; y <= Height; y++)
            DrawLine(
                new Vector2(0, y * GameStore.TILE_SIZE),
                new Vector2(Width * GameStore.TILE_SIZE, y * GameStore.TILE_SIZE),
                Colors.White,
                1f
            );
    }

    private static void RegisterTile<T>(PackedScene scene)
        where T : BaseTile => tile_scenes[typeof(T)] = scene;

    /// Places a tile at the given position.
    public T PlaceTile<T>(Vector2I pos)
        where T : BaseTile
    {
        if (!tile_scenes.TryGetValue(typeof(T), out var scene))
            throw new Exception($"No scene registered for {typeof(T).Name}");

        if (occupied_tiles.ContainsKey(pos))
            RemoveTile(pos);

        var tile = scene.Instantiate<T>();
        AddChild(tile);
        tile.GlobalPosition = GridToWorld(pos);
        tile.GridPosition = pos;
        occupied_tiles[pos] = tile;
        empty_tiles.Remove(pos);
        return tile;
    }

    /// Removes the tile at the given position.
    public void RemoveTile(Vector2I pos)
    {
        if (!occupied_tiles.TryGetValue(pos, out var tile))
            return;
        tile.QueueFree();
        occupied_tiles.Remove(pos);
        empty_tiles.Add(pos);
    }

    /// Converts a grid position to a world position.
    public Vector2 GridToWorld(Vector2I pos)
    {
        return GlobalPosition
            + new Vector2(
                pos.X * GameStore.TILE_SIZE + GameStore.TILE_SIZE / 2f,
                pos.Y * GameStore.TILE_SIZE + GameStore.TILE_SIZE / 2f
            );
    }

    /// Converts a world position to a grid position.
    public Vector2I WorldToGrid(Vector2 pos) =>
        new Vector2I((int)(pos.X / GameStore.TILE_SIZE), (int)(pos.Y / GameStore.TILE_SIZE));

    /// Get the tile at a grid position.
    public BaseTile GetTileAt(Vector2I pos) =>
        occupied_tiles.TryGetValue(pos, out var tile) ? tile : null;

    /// Get the tile at a world position.
    public BaseTile GetTileAt(Vector2 pos) => GetTileAt(WorldToGrid(pos));

    /// Get all tiles of a given type.
    public T[] GetTilesOfType<T>()
        where T : BaseTile
    {
        var tiles = new List<T>();
        foreach (var (pos, tile) in occupied_tiles)
            if (tile is T)
                tiles.Add((T)tile);
        return tiles.ToArray();
    }

    /// Get a random tile world position.
    public Vector2 GetRandomTilePosition() =>
        GridToWorld(new Vector2I((int)GD.RandRange(0, Width), (int)GD.RandRange(0, Height)));

    /// Get a random empty tile world position.
    public Vector2I? GetRandomEmptyTile()
    {
        if (empty_tiles.Count == 0)
            return null;
        return empty_tiles[(int)GD.Randi() % empty_tiles.Count];
    }

    /// Get closest tile of type T to a given world position.
    public T GetClosestTileOfType<T>(Vector2 pos)
        where T : BaseTile
    {
        if (GetTilesOfType<T>().Length == 0)
            return null;

        var closest = GetTilesOfType<T>()[0];
        float closestDistance = pos.DistanceSquaredTo(GridToWorld(closest.GridPosition));
        foreach (var tile in GetTilesOfType<T>())
        {
            float distance = pos.DistanceSquaredTo(GridToWorld(tile.GridPosition));
            if (distance < closestDistance)
            {
                closest = tile;
                closestDistance = distance;
            }
        }
        return closest;
    }
}
