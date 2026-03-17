using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class Grid : Node2D
{
    public Dictionary<Vector2I, BaseTile> Tiles = new();
    public Dictionary<Vector2I, BaseGridObject> Objects = new();

    public override void _Ready()
    {
        Services.Register(this);

        if (GameStore.Save.Tiles.Count > 0)
            LoadFrom(GameStore.Save);
        else
            BuildDefault();

        Services.Get<Tilemap>().Update(Tiles.Values);
    }

    // --- Default Layout ---

    private void BuildDefault()
    {
        for (int x = 0; x < 5; x++)
        for (int y = 0; y < 5; y++)
            PlaceTile<GreenTile>(new Vector2I(x, y));
        PlaceTile<GreenTile>(new Vector2I(5, 2));

        PlaceObject<HiveGridObject>(new Vector2I(2, 2), out var hive);

        Vector2I[] flowers = [new(0, 1), new(4, 0), new(1, 4), new(3, 3)];
        foreach (var pos in flowers)
            PlaceObject<BaseFlower>(pos, out _);
    }

    // --- Persistence ---

    private void LoadFrom(SaveData save)
    {
        foreach (var saved in save.Tiles)
        {
            var scene = GD.Load<PackedScene>($"res://objects/tiles/{saved.Type}.tscn");
            if (scene == null)
            {
                GD.PushError($"[Grid] Missing tile scene: {saved.Type}");
                continue;
            }
            var pos = new Vector2I(saved.X, saved.Y);
            var tile = scene.Instantiate<BaseTile>();
            AddChild(tile);
            tile.GlobalPosition = GridToWorld(pos);
            tile.GridPosition = pos;
            Tiles[pos] = tile;
        }

        foreach (var saved in save.Objects)
        {
            var scene = GD.Load<PackedScene>($"res://objects/grid/{saved.Type}.tscn");
            if (scene == null)
            {
                GD.PushError($"[Grid] Missing object scene: {saved.Type}");
                continue;
            }
            var pos = new Vector2I(saved.X, saved.Y);
            var obj = scene.Instantiate<BaseGridObject>();
            AddChild(obj);
            obj.GlobalPosition = GridToWorld(pos);
            obj.GridPosition = pos;
            Objects[pos] = obj;

            if (obj is HiveGridObject hive)
            {
                var savedHive = save.Hives.FirstOrDefault(h => h.X == pos.X && h.Y == pos.Y);
                // if (savedHive != null)
                //     hive.BeeCount = savedHive.BeeCount;
            }
        }

        SyncTilemap();
    }

    // --- Tile Layer ---

    private void SyncTilemap() => Services.Get<Tilemap>().Update(Tiles.Values);

    /// Core placement logic, given an already-instantiated tile.
    private bool PlaceTileInstance(BaseTile tile, Vector2I pos, out string? failMessage)
    {
        if (Tiles.Count > 0 && !HasAdjacentTile(pos))
        {
            tile.QueueFree();
            failMessage = $"No adjacent tile at {pos}";
            return false;
        }
        if (Tiles.ContainsKey(pos))
            RemoveTile(pos);

        AddChild(tile);
        tile.GlobalPosition = GridToWorld(pos);
        tile.GridPosition = pos;
        Tiles[pos] = tile;
        SyncTilemap();
        failMessage = null;
        return true;
    }

    /// Place a tile by type.
    public bool PlaceTile<T>(Vector2I pos, out T? result, out string? failMessage)
        where T : BaseTile
    {
        result = null;
        var scene = GD.Load<PackedScene>($"res://objects/tiles/{typeof(T).Name}.tscn");
        if (scene == null)
        {
            failMessage = $"No scene for {typeof(T).Name}";
            return false;
        }
        result = scene.Instantiate<T>();
        return PlaceTileInstance(result, pos, out failMessage);
    }

    public bool PlaceTile<T>(Vector2I pos, out T? result)
        where T : BaseTile => PlaceTile<T>(pos, out result, out _);

    public bool PlaceTile<T>(Vector2I pos)
        where T : BaseTile => PlaceTile<T>(pos, out _, out _);

    /// Place a tile from a packed scene resource.
    public bool PlaceTile(PackedScene scene, Vector2I pos, out string? failMessage)
    {
        var instance = scene.Instantiate();
        if (instance is not BaseTile tile)
        {
            instance.QueueFree();
            failMessage = "Scene is not a BaseTile";
            return false;
        }
        return PlaceTileInstance(tile, pos, out failMessage);
    }

    public bool PlaceTile(PackedScene scene, Vector2I pos) => PlaceTile(scene, pos, out _);

    public bool RemoveTile(Vector2I pos, out string? failMessage)
    {
        if (!Tiles.TryGetValue(pos, out var tile))
        {
            failMessage = $"No tile at {pos}";
            return false;
        }
        RemoveObject(pos);
        tile.QueueFree();
        Tiles.Remove(pos);
        SyncTilemap();
        failMessage = null;
        return true;
    }

    public bool RemoveTile(Vector2I pos) => RemoveTile(pos, out _);

    public bool HasTile(Vector2I pos) => Tiles.ContainsKey(pos);

    public BaseTile? GetTileAt(Vector2I pos) => Tiles.TryGetValue(pos, out var t) ? t : null;

    // --- Object Layer ---

    public bool PlaceObject<T>(Vector2I pos, out T? result, out string? failMessage)
        where T : BaseGridObject
    {
        result = null;
        if (!Tiles.ContainsKey(pos))
        {
            failMessage = $"No tile at {pos}";
            return false;
        }
        if (Objects.ContainsKey(pos))
        {
            failMessage = $"Object already at {pos}";
            return false;
        }
        var scene = GD.Load<PackedScene>($"res://objects/grid/{typeof(T).Name}.tscn");
        if (scene == null)
        {
            failMessage = $"No scene for {typeof(T).Name}";
            return false;
        }

        result = scene.Instantiate<T>();
        AddChild(result);
        result.GlobalPosition = GridToWorld(pos);
        result.GridPosition = pos;
        Objects[pos] = result;
        failMessage = null;
        return true;
    }

    public bool PlaceObject<T>(Vector2I pos, out T? result)
        where T : BaseGridObject => PlaceObject<T>(pos, out result, out _);

    public bool PlaceObject<T>(Vector2I pos)
        where T : BaseGridObject => PlaceObject<T>(pos, out _, out _);

    public bool RemoveObject(Vector2I pos, out string? failMessage)
    {
        if (!Objects.TryGetValue(pos, out var obj))
        {
            failMessage = $"No object at {pos}";
            return false;
        }
        obj.QueueFree();
        Objects.Remove(pos);
        failMessage = null;
        return true;
    }

    public bool RemoveObject(Vector2I pos) => RemoveObject(pos, out _);

    public bool HasObject(Vector2I pos) => Objects.ContainsKey(pos);

    public BaseGridObject? GetObjectAt(Vector2I pos) =>
        Objects.TryGetValue(pos, out var o) ? o : null;

    public BaseGridObject? GetObjectAt(Vector2 pos) => GetObjectAt(WorldToGrid(pos));

    public BaseTile? GetTileOfObject(BaseGridObject obj) => GetTileAt(obj.GridPosition);

    public BaseGridObject? GetObjectOfTile(BaseTile tile) => GetObjectAt(tile.GridPosition);

    public IEnumerable<BaseTile> GetEmptyTiles() =>
        Tiles.Where(kv => !Objects.ContainsKey(kv.Key)).Select(kv => kv.Value);

    public T[] GetObjectsOfType<T>()
        where T : BaseGridObject => Objects.Values.OfType<T>().ToArray();

    public T[] GetTilesOfType<T>()
        where T : BaseTile => Tiles.Values.OfType<T>().ToArray();

    public T? GetClosestObjectOfType<T>(Vector2 pos)
        where T : BaseGridObject
    {
        T? closest = null;
        float best = float.MaxValue;
        foreach (var obj in GetObjectsOfType<T>())
        {
            float d = pos.DistanceSquaredTo(GridToWorld(obj.GridPosition));
            if (d < best)
            {
                best = d;
                closest = obj;
            }
        }
        return closest;
    }

    public T? GetClosestTileOfType<T>(Vector2 pos)
        where T : BaseTile
    {
        T? closest = null;
        float best = float.MaxValue;
        foreach (var tile in GetTilesOfType<T>())
        {
            float d = pos.DistanceSquaredTo(GridToWorld(tile.GridPosition));
            if (d < best)
            {
                best = d;
                closest = tile;
            }
        }
        return closest;
    }

    // --- Helpers ---

    public bool HasAdjacentTile(Vector2I pos) =>
        Tiles.ContainsKey(pos + Vector2I.Left)
        || Tiles.ContainsKey(pos + Vector2I.Right)
        || Tiles.ContainsKey(pos + Vector2I.Up)
        || Tiles.ContainsKey(pos + Vector2I.Down);

    public Vector2 GridToWorld(Vector2I pos) =>
        GlobalPosition
        + new Vector2(
            pos.X * GameStore.TILE_SIZE + GameStore.TILE_SIZE / 2f,
            pos.Y * GameStore.TILE_SIZE + GameStore.TILE_SIZE / 2f
        );

    public Vector2I WorldToGrid(Vector2 pos)
    {
        var local = pos - GlobalPosition;
        return new Vector2I(
            (int)(local.X / GameStore.TILE_SIZE),
            (int)(local.Y / GameStore.TILE_SIZE)
        );
    }

    public Vector2 GetRandomTilePosition()
    {
        var all = Tiles.Keys.ToArray();
        return GridToWorld(all[(int)GD.Randi() % all.Length]);
    }

    public BaseTile? GetRandomEmptyTile()
    {
        var empty = GetEmptyTiles().ToArray();
        return empty.Length == 0 ? null : empty[(int)GD.Randi() % empty.Length];
    }
}
