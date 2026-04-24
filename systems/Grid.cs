using System;
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

        SignalBus.Instance.GameLoaded += () => OnGameLoaded();

        Services.Get<Tilemap>().Update(tiles.Values);
    }

    private void OnGameLoaded()
    {
        if (GameStore.Save.Tiles.Count > 0)
            LoadFrom(GameStore.Save);
        else
            BuildDefault();
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.GridLoaded);
    }

    // --- Default Layout ---

    private void BuildDefault()
    {
        for (int x = -2; x <= 2; x++)
        for (int y = -2; y <= 2; y++)
            PlaceTile<DirtTile>(new Vector2I(x, y));

        PlaceObject<Hive>(new Vector2I(0, 0), out var hive);
    }

    // --- Persistence ---

    public void Snapshot()
    {
        GameStore.Save.Tiles = tiles
            .Values.GroupBy(t => t.GetType().Name)
            .Select(g =>
            {
                var byRow = g.GroupBy(t => t.GridPosition.Y).OrderBy(row => row.Key);
                var spans = new List<int[]>();
                foreach (var row in byRow)
                {
                    var xs = row.Select(t => t.GridPosition.X).OrderBy(x => x).ToList();
                    int start = xs[0],
                        prev = xs[0];
                    for (int i = 1; i < xs.Count; i++)
                    {
                        if (xs[i] != prev + 1)
                        {
                            spans.Add([start, prev, row.Key]);
                            start = xs[i];
                        }
                        prev = xs[i];
                    }
                    spans.Add([start, prev, row.Key]);
                }
                return new SavedTileGroup { Type = g.Key, Spans = spans };
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

        GameStore.Save.Hives = GetObjectsOfType<Hive>()
            .Select(h => new SavedHive
            {
                X = h.GridPosition.X,
                Y = h.GridPosition.Y,
                BeeCounts = h.GetBeeCounts(),
            })
            .ToList();
    }

    private void LoadFrom(SaveData save)
    {
        foreach (var group in save.Tiles)
        {
            var scene = GD.Load<PackedScene>($"res://objects/tiles/{group.Type}.tscn");
            if (scene == null)
            {
                GD.PushError($"[Grid] Missing tile scene: {group.Type}");
                continue;
            }
            foreach (var span in group.Spans)
                for (int x = span[0]; x <= span[1]; x++)
                {
                    var pos = new Vector2I(x, span[2]);
                    var tile = scene.Instantiate<BaseTile>();
                    AddChild(tile);
                    tile.GlobalPosition = GridToWorld(pos);
                    tile.GridPosition = pos;
                    tiles[pos] = tile;
                }
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
            obj.Placed = true;
            obj.GridPosition = pos;
            objects[pos] = obj;
        }

        SyncTilemap();
    }

    // --- Tile Layer ---

    private void SyncTilemap() => Services.Get<Tilemap>().Update(tiles.Values);

    /// Core placement logic, given an already-instantiated tile.
    private bool PlaceTile(BaseTile tile, Vector2I pos, out FailMessage? failMessage)
    {
        if (!PlacementValid(pos, out failMessage))
            return false;

        AddChild(tile);
        tile.GlobalPosition = GridToWorld(pos);
        tile.GridPosition = pos;
        tiles[pos] = tile;
        SyncTilemap();
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.TilePlaced, tile);
        failMessage = null;
        return true;
    }

    /// Place a tile by type.
    public bool PlaceTile<T>(Vector2I pos, out T? result, out FailMessage? failMessage)
        where T : BaseTile
    {
        PackedScene? scene = GD.Load<PackedScene>($"res://objects/tiles/{typeof(T).Name}.tscn");
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
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.TileRemoved, tile);
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
            failMessage = new FailMessage(
                $"Object already at {pos}.",
                "Tile already has an object!"
            );
            return false;
        }

        AddChild(obj);
        obj.GlobalPosition = GridToWorld(pos);
        obj.GridPosition = pos;
        objects[pos] = obj;
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.GridObjectPlaced, obj);
        obj.Placed = true;
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
        if (obj is Hive hive)
        {
            foreach (Bee bee in hive.Bees)
                bee.Remove();
        }
        obj.QueueFree();
        objects.Remove(pos);
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.GridObjectRemoved, obj);
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

    public int GetObjectCountOfType(Type t) => objects.Values.Count(o => o.GetType() == t);

    public int GetTileCountOfType(Type t) => tiles.Values.Count(tile => tile.GetType() == t);

    // True if any tile (excluding `exclude`) is within `distance` of `pos`.
    private bool HasTileWithin(Vector2I pos, int distance, HashSet<Vector2I>? exclude = null)
    {
        foreach (Vector2I tile in tiles.Keys)
        {
            if (exclude != null && exclude.Contains(tile))
                continue;
            if (Mathf.Abs(tile.X - pos.X) <= distance && Mathf.Abs(tile.Y - pos.Y) <= distance)
                return true;
        }
        return false;
    }

    /// Check if placement of a tile would leave the grid in a valid state.
    private bool PlacementValid(Vector2I pos, out FailMessage? failMessage)
    {
        if (tiles.Count < 1)
        {
            failMessage = null;
            return true;
        }
        if (tiles.ContainsKey(pos))
        {
            failMessage = new FailMessage(
                $"Tile at {pos} already exists.",
                "Another tile is there!"
            );
            return false;
        }
        if (!HasTileWithin(pos, GameStore.ValidTileDistance))
        {
            failMessage = new FailMessage(
                $"No nearby tiles within {GameStore.ValidTileDistance} of {pos}.",
                "No nearby tiles!"
            );
            return false;
        }
        failMessage = null;
        return true;
    }

    /// Check if removal of a tile would leave the grid in a valid state.
    private bool RemovalValid(Vector2I pos, out FailMessage? failMessage)
    {
        if (!tiles.ContainsKey(pos))
        {
            failMessage = new FailMessage($"No tile at {pos}.", "No tile there!");
            return false;
        }
        if (tiles.Count <= 2)
        {
            failMessage = null;
            return true;
        }

        HashSet<Vector2I> exclude = [pos];
        int vtd = GameStore.ValidTileDistance;
        foreach (Vector2I tile in tiles.Keys)
        {
            if (tile == pos)
                continue;
            if (Mathf.Abs(tile.X - pos.X) > vtd || Mathf.Abs(tile.Y - pos.Y) > vtd)
                continue;

            if (!HasTileWithin(tile, vtd, exclude))
            {
                failMessage = new FailMessage(
                    $"Removing tile at {pos} would isolate {tile}.",
                    "Removing this tile isolates other tiles!"
                );
                return false;
            }
        }

        failMessage = null;
        return true;
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
        if (all.Length == 0)
            return GlobalPosition;
        return GridToWorld(all[GD.Randi() % (uint)all.Length]);
    }

    public BaseTile? GetRandomEmptyTile()
    {
        var empty = GetEmptyTiles().ToArray();
        return empty.Length == 0 ? null : empty[(int)GD.Randi() % empty.Length];
    }
}
