using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class Grid : Node2D
{
    private Dictionary<Vector2I, BaseTile> _grid = new();
    public int Width { get; set; } = 10;
    public int Height { get; set; } = 10;

    private static Dictionary<Type, PackedScene> _scenes = new();

    public override void _Ready()
    {
        // register tile service
        Services.Register(this);

        // RegisterTile<HiveTile>(GD.Load<PackedScene>("res://objects/HiveTile.tscn"));
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
        where T : BaseTile => _scenes[typeof(T)] = scene;

    /// Places a tile at the given position.
    public T PlaceTile<T>(Vector2I pos)
        where T : BaseTile
    {
        if (!_scenes.TryGetValue(typeof(T), out var scene))
            throw new Exception($"No scene registered for {typeof(T).Name}");

        if (_grid.ContainsKey(pos))
            RemoveTile(pos);

        var tile = scene.Instantiate<T>();
        AddChild(tile);
        tile.GlobalPosition = GridToWorld(pos);
        tile.GridPosition = pos;
        _grid[pos] = tile;
        return tile;
    }

    /// Removes the tile at the given position.
    public void RemoveTile(Vector2I pos)
    {
        if (!_grid.TryGetValue(pos, out var tile))
            return;
        tile.QueueFree();
        _grid.Remove(pos);
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
}
