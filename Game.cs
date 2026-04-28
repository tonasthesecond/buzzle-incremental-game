using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Game : GameSystem
{
    public Node2D GameLayer { get; private set; } = null!;
    public Node2D UpgradeLayer { get; private set; } = null!;
    public CanvasLayer UILayer { get; private set; } = null!;
    public BaseButton ShowUpgradesButton { get; private set; } = null!;
    public Control PlacementMenu { get; private set; } = null!;
    public Control CollapseButton { get; private set; } = null!;
    public SelectContainer ObjectsSelect { get; private set; } = null!;
    public SelectContainer TilesSelect { get; private set; } = null!;
    public SelectContainer BeesSelect { get; private set; } = null!;
    public PlacementSystem PlacementSystem { get; private set; } = null!;
    public UpgradeTree UpgradeTree { get; private set; } = null!;

    [Export]
    public bool AutoSave { get; set; } = true;

    [Export]
    public bool DebugMode { get; set; } = false;

    [Export]
    public int AutoSaveInterval { get; set; } = 3 * 60 * 1000;

    private Timer? autoSaveTimer;

    public override void _Ready()
    {
        // get nodes
        GameLayer = GetNode<Node2D>("%GameLayer");
        UpgradeLayer = GetNode<Node2D>("%UpgradeLayer");
        UILayer = GetNode<CanvasLayer>("%UILayer");
        ShowUpgradesButton = GetNode<BaseButton>("%ShowUpgradesButton");
        PlacementMenu = GetNode<Control>("%PlacementMenu");
        CollapseButton = UILayer.GetNode<Control>("CollapseButton");
        ObjectsSelect = UILayer.GetNode<SelectContainer>("%ObjectsSelectContainer");
        TilesSelect = UILayer.GetNode<SelectContainer>("%TilesSelectContainer");
        BeesSelect = UILayer.GetNode<SelectContainer>("%BeesSelectContainer");
        PlacementSystem = GetNode<PlacementSystem>("%PlacementSystem");
        UpgradeTree = GetNode<UpgradeTree>("%UpgradeTree");

        // connect signals
        ShowUpgradesButton.Pressed += onUpgradesButtonPressed;

        // setup
        GameLayer.Show();
        GameLayer.GetNode<Camera>("Camera").MakeCurrent();

        // load game
        GameStore.LoadGame();

        // start autosave
        if (AutoSave)
        {
            autoSaveTimer = new Timer();
            autoSaveTimer.OneShot = false;
            AddChild(autoSaveTimer);
            autoSaveTimer.Timeout += GameStore.SaveGame;
            autoSaveTimer.Start(AutoSaveInterval);
        }
        if (DebugMode)
        {
            GD.Print("Debug Mode: ON");
            PlacementSystem.FreePlace = true;
            UpgradeTree.ShowAllUpgrades = true;
        }
        else
        {
            GD.Print("Debug Mode: OFF");
            PlacementSystem.FreePlace = false;
            UpgradeTree.ShowAllUpgrades = false;
        }
    }

    private void onUpgradesButtonPressed()
    {
        if (UpgradeLayer.Visible)
        {
            UpgradeLayer.Hide();
            GameLayer.Show();
            PlacementMenu.Show();
            CollapseButton.Show();
            GameLayer.GetNode<Camera>("Camera").MakeCurrent();
        }
        else
        {
            ObjectsSelect.Reset();
            TilesSelect.Reset();
            BeesSelect.Reset();
            UpgradeLayer.Show();
            GameLayer.Hide();
            PlacementMenu.Hide();
            CollapseButton.Hide();
            UpgradeLayer.GetNode<Camera>("Camera").MakeCurrent();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("save_game"))
        {
            GD.Print("Saved Game!");
            GameStore.SaveGame();
        }
        if (Input.IsActionJustPressed("test"))
        {
            GD.Print($"Total: {Services.Get<HoneyTracker>().GetAllTime()}");
            GD.Print(Services.Get<HoneyTracker>().samples.Count);
            Dictionary<string, float> bySource = Services.Get<HoneyTracker>().GetHPSBySource();
            GD.Print($"By Source:");
            GD.Print($"Total: {bySource.Values.Sum()}");
            float total = bySource.Values.Sum();
            foreach (var (source, hps) in bySource)
                GD.Print($"{source}: {(total > 0 ? hps / total : 0):P2}");
        }
    }
}
