using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Tilemap : TileMapLayer
{
    private const int TerrainSet = 0;
    private const int Terrain = 0;

    private static readonly Dictionary<Type, int> TileSources = new()
    {
        { typeof(GreenTile), 0 },
        // { typeof(DirtTile), 1 },
    };

    public override void _EnterTree()
    {
        Services.Register(this);
    }

    /// Update the tilemap, given a list of tiles.
    public void Update(IEnumerable<BaseTile> tiles)
    {
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

        UpdateBottomEdges(tiles);
    }

    /// Add the bottom strip of cells to the tilemap.
    private void UpdateBottomEdges(IEnumerable<BaseTile> tiles)
    {
        var positions = new HashSet<Vector2I>(tiles.Select(t => t.GridPosition));

        // find all bottom-edge columns grouped by row
        var bottomEdges = positions
            .Where(p => !positions.Contains(p + Vector2I.Down))
            .OrderBy(p => p.Y)
            .ThenBy(p => p.X)
            .ToList();

        // clear old edge tiles (one row below any existing edge)
        foreach (var p in bottomEdges)
            EraseCell(p + Vector2I.Down);

        // group into contiguous horizontal runs per row
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
            }
        }
    }
}
