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
    public CanvasLayer EndLayer { get; private set; } = null!;
    public Container EndingContentContainer { get; private set; } = null!;

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
        EndLayer = GetNode<CanvasLayer>("%EndLayer");
        EndingContentContainer = GetNode<Container>("%EndingContentContainer");

        // connect signals
        ShowUpgradesButton.Pressed += onUpgradesButtonPressed;
        SignalBus.Instance.RainbowPlaced += onRainbowPlaced;

        // setup
        GameLayer.Show();
        EndLayer.Hide();
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

    private void onRainbowPlaced(Rainbow rainbow)
    {
        EndLayer.Show();
        Camera camera = GameLayer.GetNode<Camera>("Camera");
        camera.ControlsEnabled = false;
        camera.SetTarget(rainbow.GlobalPosition);
        camera.SetZoom(GameStore.GameEndStartZoom);

        Tween tween = CreateTween();
        tween.TweenMethod(
            Callable.From((float v) => GameEndJob.SpeedScale = v),
            GameEndJob.SpeedScale,
            GameStore.RainbowSpeedScaleMax,
            GameStore.GameEndAnimationTime
        );
        tween.SetEase(Tween.EaseType.In);
        tween.SetTrans(Tween.TransitionType.Expo);

        UILayer.Hide();

        Tween cameraTween = CreateTween();
        cameraTween.TweenMethod(
            Callable.From((float v) => camera.SetZoom(v)),
            GameStore.GameEndStartZoom,
            GameStore.GameEndEndZoom,
            GameStore.GameEndAnimationTime
        );

        ColorRect whiteScreen = GetNode<ColorRect>("%WhiteScreen");
        whiteScreen.Show();
        Tween whiteTween = null!;
        GetTree().CreateTimer(GameStore.WhiteScreenDelayTime).Timeout += () =>
        {
            whiteTween = CreateTween();
            whiteTween.TweenProperty(whiteScreen, "modulate:a", 1f, GameStore.WhiteScreenFadeTime);
            whiteTween.SetEase(Tween.EaseType.In);
            whiteTween.SetTrans(Tween.TransitionType.Quad);
            whiteTween.Finished += () =>
            {
                Services.Get<BeeSystem>().ResetBees();
                Tween containerTween = CreateTween();
                containerTween.TweenProperty(EndingContentContainer, "modulate:a", 1f, 1f);
                containerTween.SetEase(Tween.EaseType.In);
                containerTween.SetTrans(Tween.TransitionType.Quad);
            };
        };
    }
}
