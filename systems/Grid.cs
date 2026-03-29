using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class Grid : Node2D
{
    private Dictionary<Vector2I, BaseTile> tiles = new();
    private Dictionary<Vector2I, BaseGridObject> objects = new();

    public override void _Ready()
    {
        Services.Register(this);

        if (GameStore.Save.Tiles.Count > 0)
            LoadFrom(GameStore.Save);
        else
            BuildDefault();

        Services.Get<Tilemap>().Update(tiles.Values);
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

        // spawn bees
        for (int i = 0; i < 5; i++)
            Services.Get<BeeSystem>().SpawnBee(hive!);
    }

    // --- Persistence ---

    public void Snapshot()
    {
        GameStore.Save.Tiles = tiles
            .Values.Select(t => new SavedTile
            {
                X = t.GridPosition.X,
                Y = t.GridPosition.Y,
                Type = t.GetType().Name,
            })
            .ToList();

        GameStore.Save.Objects = objects
            .Values.Select(o => new SavedObject
            {
                X = o.GridPosition.X,
                Y = o.GridPosition.Y,
                Type = o.GetType().Name,
            })
            .ToList();

        GameStore.Save.Hives = GetObjectsOfType<HiveGridObject>()
            .Select(h => new SavedHive
            {
                X = h.GridPosition.X,
                Y = h.GridPosition.Y,
                BeeCount = h.BeeCount,
            })
            .ToList();
    }

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
            tiles[pos] = tile;
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
            objects[pos] = obj;

            // if (obj is HiveGridObject hive)
            // {
            //     var savedHive = save.Hives.FirstOrDefault(h => h.X == pos.X && h.Y == pos.Y);
            //     if (savedHive != null)
            //         hive.BeeCount = savedHive.BeeCount;
            // }
        }

        SyncTilemap();
    }

    // --- Tile Layer ---

    private void SyncTilemap() => Services.Get<Tilemap>().Update(tiles.Values);

    /// Core placement logic, given an already-instantiated tile.
    private bool PlaceTile(BaseTile tile, Vector2I pos, out FailMessage? failMessage)
    {
        if (tiles.Count > 0 && !HasAdjacentTile(pos))
        {
            tile.QueueFree();
            failMessage = new FailMessage($"No adjacent tile at {pos}.", "No adjacent tile there!");
            return false;
        }
        if (tiles.ContainsKey(pos))
            RemoveTile(pos);

        AddChild(tile);
        tile.GlobalPosition = GridToWorld(pos);
        tile.GridPosition = pos;
        tiles[pos] = tile;
        SyncTilemap();
        failMessage = null;
        return true;
    }

    /// Place a tile by type.
    public bool PlaceTile<T>(Vector2I pos, out T? result, out FailMessage? failMessage)
        where T : BaseTile
    {
        result = null;
        var scene = GD.Load<PackedScene>($"res://objects/tiles/{typeof(T).Name}.tscn");
        if (scene == null)
        {
            failMessage = new FailMessage($"No scene for {typeof(T).Name}.", "");
            return false;
        }
        result = scene.Instantiate<T>();
        return PlaceTile(result, pos, out failMessage);
    }

    public bool PlaceTile<T>(Vector2I pos, out T? result)
        where T : BaseTile => PlaceTile<T>(pos, out result, out _);

    public bool PlaceTile<T>(Vector2I pos)
        where T : BaseTile => PlaceTile<T>(pos, out _, out _);

    /// Place a tile from a packed scene resource.
    public bool PlaceTile(PackedScene scene, Vector2I pos, out FailMessage? failMessage)
    {
        var instance = scene.Instantiate();
        if (instance is not BaseTile tile)
        {
            instance.QueueFree();
            failMessage = new FailMessage("Scene is not a BaseTile.", "");
            return false;
        }
        return PlaceTile(tile, pos, out failMessage);
    }

    public bool PlaceTile(PackedScene scene, Vector2I pos) => PlaceTile(scene, pos, out _);

    public bool RemoveTile(Vector2I pos, out FailMessage? failMessage)
    {
        if (!tiles.TryGetValue(pos, out var tile))
        {
            failMessage = new FailMessage($"No tile at {pos}.", "No tile there!");
            return false;
        }
        if (objects.ContainsKey(pos))
        {
            failMessage = new FailMessage($"Tile at {pos} is occupied.", "Tile is occupied!");
            return false;
        }
        RemoveObject(pos);
        tile.QueueFree();
        tiles.Remove(pos);
        SyncTilemap();
        failMessage = null;
        return true;
    }

    public bool RemoveTile(Vector2I pos) => RemoveTile(pos, out _);

    public bool HasTile(Vector2I pos) => tiles.ContainsKey(pos);

    public BaseTile? GetTileAt(Vector2I pos) => tiles.TryGetValue(pos, out var t) ? t : null;

    // --- Object Layer ---

    /// Core placement logic, given an already-instantiated object.
    private bool PlaceObject(BaseGridObject obj, Vector2I pos, out FailMessage? failMessage)
    {
        if (!tiles.ContainsKey(pos))
        {
            obj.QueueFree();
            failMessage = new FailMessage($"No tile at {pos}.", "No tile there!");
            return false;
        }
        if (objects.ContainsKey(pos))
        {
            obj.QueueFree();
            failMessage = new FailMessage($"Object already at {pos}.", "Tile has an object!");
            return false;
        }

        AddChild(obj);
        obj.GlobalPosition = GridToWorld(pos);
        obj.GridPosition = pos;
        objects[pos] = obj;
        failMessage = null;
        return true;
    }

    /// Place an object by type.
    public bool PlaceObject<T>(Vector2I pos, out T? result, out FailMessage? failMessage)
        where T : BaseGridObject
    {
        result = null;
        var scene = GD.Load<PackedScene>($"res://objects/grid/{typeof(T).Name}.tscn");
        if (scene == null)
        {
            failMessage = new FailMessage($"No scene for {typeof(T).Name}.", "");
            return false;
        }
        result = scene.Instantiate<T>();
        return PlaceObject(result, pos, out failMessage);
    }

    public bool PlaceObject<T>(Vector2I pos, out T? result)
        where T : BaseGridObject => PlaceObject<T>(pos, out result, out _);

    public bool PlaceObject<T>(Vector2I pos)
        where T : BaseGridObject => PlaceObject<T>(pos, out _, out _);

    /// Place an object from a packed scene resource.
    public bool PlaceObject(PackedScene scene, Vector2I pos, out FailMessage? failMessage)
    {
        var instance = scene.Instantiate();
        if (instance is not BaseGridObject obj)
        {
            instance.QueueFree();
            failMessage = new FailMessage("Scene is not a BaseGridObject.", "");
            return false;
        }
        return PlaceObject(obj, pos, out failMessage);
    }

    public bool PlaceObject(PackedScene scene, Vector2I pos) => PlaceObject(scene, pos, out _);

    public bool RemoveObject(Vector2I pos, out FailMessage? failMessage)
    {
        if (!objects.TryGetValue(pos, out var obj))
        {
            failMessage = new FailMessage($"No object at {pos}.", "No object there!");
            return false;
        }
        obj.QueueFree();
        objects.Remove(pos);
        failMessage = null;
        return true;
    }

    public bool RemoveObject(Vector2I pos) => RemoveObject(pos, out _);

    public bool HasObject(Vector2I pos) => objects.ContainsKey(pos);

    // --- Queries ---
    public IEnumerable<BaseTile> GetTiles() => tiles.Values;

    public IEnumerable<BaseGridObject> GetObjects() => objects.Values;

    public BaseGridObject? GetObjectAt(Vector2I pos) =>
        objects.TryGetValue(pos, out var o) ? o : null;

    public BaseGridObject? GetObjectAt(Vector2 pos) => GetObjectAt(WorldToGrid(pos));

    public BaseTile? GetTileOfObject(BaseGridObject obj) => GetTileAt(obj.GridPosition);

    public BaseGridObject? GetObjectOfTile(BaseTile tile) => GetObjectAt(tile.GridPosition);

    public IEnumerable<BaseTile> GetEmptyTiles() =>
        tiles.Where(kv => !objects.ContainsKey(kv.Key)).Select(kv => kv.Value);

    public T[] GetObjectsOfType<T>()
        where T : BaseGridObject => objects.Values.OfType<T>().ToArray();

    public T[] GetTilesOfType<T>()
        where T : BaseTile => tiles.Values.OfType<T>().ToArray();

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
        tiles.ContainsKey(pos + Vector2I.Left)
        || tiles.ContainsKey(pos + Vector2I.Right)
        || tiles.ContainsKey(pos + Vector2I.Up)
        || tiles.ContainsKey(pos + Vector2I.Down);

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
        var all = tiles.Keys.ToArray();
        return GridToWorld(all[(int)GD.Randi() % all.Length]);
    }

    public BaseTile? GetRandomEmptyTile()
    {
        var empty = GetEmptyTiles().ToArray();
        return empty.Length == 0 ? null : empty[(int)GD.Randi() % empty.Length];
    }
}
