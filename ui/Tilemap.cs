using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Tilemap : TileMapLayer
{
    [Export]
    public bool DrawBottomStrips = true;

    private const int TerrainSet = 0;
    private const int Terrain = 0;
    private bool previewMode = false;
    private Vector2I? ghostPos = null;
    private List<BaseTile> currentTiles = new();
    private HashSet<Vector2I> realTiles = new();
    private HashSet<Vector2I> lastBottomEdgeCells = new();

    // maps tile types to tile source ids
    private static readonly Dictionary<Type, int> TileSources = new() { { typeof(GreenTile), 1 } };

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
                    ghostPos = null;
                    Redraw();
                    SignalBus.Instance.ResourceUnselected -= OnUnselected;
                }
                SignalBus.Instance.ResourceUnselected += OnUnselected;
            }
        };
    }

    public override void _Process(double delta)
    {
        if (!previewMode)
            return;

        var mouseGrid = LocalToMap(GetLocalMousePosition());
        if (ghostPos == mouseGrid)
            return;

        // update new ghost position
        ghostPos = mouseGrid;
        Redraw();
    }

    /// Update the tilemap, given a list of tiles.
    public void Update(IEnumerable<BaseTile> tiles)
    {
        currentTiles = tiles.ToList();
        realTiles = new HashSet<Vector2I>(currentTiles.Select(t => t.GridPosition));
        Redraw();
    }

    /// Redraw all real tiles and the ghost (if any) from scratch.
    private void Redraw()
    {
        // nuke the old tiles
        Clear();
        lastBottomEdgeCells.Clear();

        // append ghost pos if it exists
        var displayPos = ghostPos.HasValue
            ? realTiles.Append(ghostPos.Value)
            : realTiles.AsEnumerable();

        // draw the new tiles
        SetCellsTerrainConnect(
            new Godot.Collections.Array<Vector2I>(displayPos),
            TerrainSet,
            Terrain
        );

        // set the correct atlas coords for each tile
        foreach (var tile in currentTiles)
        {
            if (!TileSources.TryGetValue(tile.GetType(), out var sourceId))
                continue;
            SetCell(tile.GridPosition, sourceId, GetCellAtlasCoords(tile.GridPosition));
        }

        UpdateBottomStrips(displayPos);
    }

    private void UpdateBottomStrips(IEnumerable<Vector2I> positions)
    {
        // don't draw bottom strips if disabled
        if (!DrawBottomStrips)
            return;

        // clear previous bottom strips (already nuked by Clear(), but keep list consistent)
        foreach (var p in lastBottomEdgeCells)
            EraseCell(p);
        lastBottomEdgeCells.Clear();

        // find all the bottom edges
        var posSet = new HashSet<Vector2I>(positions);
        var bottomEdges = posSet
            .Where(p => !posSet.Contains(p + Vector2I.Down))
            .OrderBy(p => p.Y)
            .ThenBy(p => p.X)
            .ToList();

        // group bottom edges into contiguous runs
        var runs = new List<List<Vector2I>>();
        foreach (var p in bottomEdges)
        {
            var last = runs.LastOrDefault();
            if (last != null && last[^1].Y == p.Y && last[^1].X == p.X - 1)
                last.Add(p);
            else
                runs.Add(new List<Vector2I> { p });
        }

        // draw the bottom strips
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
}
