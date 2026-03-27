using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Tilemap : TileMapLayer
{
    private const int TerrainSet = 0;
    private const int Terrain = 0;
    private bool previewMode = false;
    private Vector2I? ghostPos = null;
    private HashSet<Vector2I> realTiles = new();
    private HashSet<Vector2I> lastBottomEdgeCells = new();

    private static readonly Dictionary<Type, int> TileSources = new()
    {
        { typeof(GreenTile), 0 },
        // { typeof(DirtTile), 1 },
    };

    public override void _EnterTree()
    {
        Services.Register(this);
    }

    public override void _Ready()
    {
        SignalBus.Instance.ResourceSelected += (resource) =>
        {
            if (resource is PackedScene scene && scene.Instantiate() is BaseTile)
            {
                previewMode = true;
                void OnUnselected()
                {
                    previewMode = false;
                    SignalBus.Instance.ResourceUnselected -= OnUnselected;
                }
                SignalBus.Instance.ResourceUnselected += OnUnselected;
            }
        };
    }

    public override void _Process(double delta)
    {
        if (!previewMode)
        {
            ClearGhost();
            return;
        }

        var mouseGrid = LocalToMap(GetLocalMousePosition());
        if (ghostPos == mouseGrid)
            return;

        ClearGhost();

        ghostPos = mouseGrid;
        SetCellsTerrainConnect(
            new Godot.Collections.Array<Vector2I> { mouseGrid },
            TerrainSet,
            Terrain
        );
        UpdateBottomEdges(realTiles.Append(mouseGrid));
    }

    /// Clear the ghost cell and reconnect the tiles it was connected to.
    private void ClearGhost()
    {
        if (!ghostPos.HasValue)
            return;

        var toReconnect = new[]
        {
            ghostPos.Value,
            ghostPos.Value + Vector2I.Up,
            ghostPos.Value + Vector2I.Down,
            ghostPos.Value + Vector2I.Left,
            ghostPos.Value + Vector2I.Right,
        }
            .Where(p => realTiles.Contains(p))
            .ToList();

        if (toReconnect.Count > 0)
            SetCellsTerrainConnect(
                new Godot.Collections.Array<Vector2I>(toReconnect),
                TerrainSet,
                Terrain
            );
        else
            EraseCell(ghostPos.Value);

        ghostPos = null;
        UpdateBottomEdges(realTiles);
    }

    /// Update the tilemap, given a list of tiles.
    public void Update(IEnumerable<BaseTile> tiles)
    {
        realTiles = new HashSet<Vector2I>(tiles.Select(t => t.GridPosition));

        var groups = new Dictionary<int, List<Vector2I>>();
        foreach (var tile in tiles)
        {
            if (!TileSources.TryGetValue(tile.GetType(), out var sourceId))
                continue;
            if (!groups.ContainsKey(sourceId))
                groups[sourceId] = new();
            groups[sourceId].Add(tile.GridPosition);
        }
        foreach (var (sourceId, cells) in groups)
            SetCellsTerrainConnect(
                new Godot.Collections.Array<Vector2I>(cells),
                TerrainSet,
                Terrain
            );
        UpdateBottomEdges(realTiles);
    }

    private void UpdateBottomEdges(IEnumerable<Vector2I> positions)
    {
        foreach (var p in lastBottomEdgeCells)
            EraseCell(p);
        lastBottomEdgeCells.Clear();

        var posSet = new HashSet<Vector2I>(positions);

        var bottomEdges = posSet
            .Where(p => !posSet.Contains(p + Vector2I.Down))
            .OrderBy(p => p.Y)
            .ThenBy(p => p.X)
            .ToList();

        var runs = new List<List<Vector2I>>();
        foreach (var p in bottomEdges)
        {
            var last = runs.LastOrDefault();
            if (last != null && last[^1].Y == p.Y && last[^1].X == p.X - 1)
                last.Add(p);
            else
                runs.Add(new List<Vector2I> { p });
        }

        foreach (var run in runs)
        {
            for (int i = 0; i < run.Count; i++)
            {
                var below = run[i] + Vector2I.Down;
                var atlasCoord =
                    i == 0 ? new Vector2I(2, 5)
                    : i == run.Count - 1 ? new Vector2I(4, 5)
                    : new Vector2I(3, 5);
                SetCell(below, 0, atlasCoord);
                lastBottomEdgeCells.Add(below);
            }
        }
    }

    private void UpdateBottomEdges(IEnumerable<BaseTile> tiles) =>
        UpdateBottomEdges(tiles.Select(t => t.GridPosition));
}
