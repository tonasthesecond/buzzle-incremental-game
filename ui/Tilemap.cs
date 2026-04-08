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
    private bool removeMode = false;
    private Vector2I? ghostPos = null;
    private int ghostSourceId = 0;
    private List<BaseTile> currentTiles = new();
    private HashSet<Vector2I> realTiles = new();
    private HashSet<Vector2I> lastBottomEdgeCells = new();

    // maps tile types to tile source ids
    private static readonly Dictionary<Type, int> TileSources = new()
    {
        { typeof(GreenTile), 0 },
        { typeof(PurpleTile), 1 },
    };

    public override void _EnterTree()
    {
        Services.Register(this);
    }

    public override void _Ready()
    {
        var placement = Services.Get<PlacementSystem>();
        placement.ModeChanged += (mode) =>
        {
            previewMode = false;
            previewMode =
                mode == PlacementSystem.Mode.Tile || mode == PlacementSystem.Mode.RemoveTile;
            removeMode = mode == PlacementSystem.Mode.RemoveTile;
            if (!previewMode)
            {
                ghostPos = null;
                Redraw();
            }
        };
        SignalBus.Instance.ResourceSelected += (Resource resource) =>
        {
            if (resource is PackedScene scene)
            {
                var instance = scene.Instantiate();
                if (instance is BaseTile tile)
                    ghostSourceId = TileSources.GetValueOrDefault(tile.GetType(), 0);
                instance.QueueFree();
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
            ? removeMode
                ? realTiles.Where(p => p != ghostPos.Value)
                : realTiles.Append(ghostPos.Value)
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

        // set the correct atlas coords for the ghost
        if (ghostPos.HasValue && !removeMode)
            SetCell(ghostPos.Value, ghostSourceId, GetCellAtlasCoords(ghostPos.Value));

        UpdateBottomStrips(displayPos);
    }

    private Vector2I[] leftStrips = [new Vector2I(0, 4)];
    private Vector2I[] middleStrips = [new Vector2I(1, 4)];
    private Vector2I[] rightStrips = [new Vector2I(2, 4)];
    private Vector2I[] singleStrips = [new Vector2I(3, 4)];

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
                var sourceId = GetCellSourceId(run[i]);

                Vector2I[] pool =
                    run.Count == 1 ? singleStrips
                    : i == 0 ? leftStrips
                    : i == run.Count - 1 ? rightStrips
                    : middleStrips;

                SetCell(below, sourceId, pool[(int)GD.Randi() % pool.Length]);
                lastBottomEdgeCells.Add(below);
            }
        }
    }
}
