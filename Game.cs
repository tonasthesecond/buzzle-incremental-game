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
    public BaseButton BackButton { get; private set; } = null!;
    public BaseButton TutorialButton { get; private set; } = null!;
    public Control TutorialLayer { get; private set; } = null!;
    public BaseButton TutorialCloseButton { get; private set; } = null!;

    [Export]
    public bool DebugMode { get; set; } = false;

<<<<<<< HEAD
=======
    [Export]
    public int AutoSaveInterval { get; set; } = 2 * 60;

    private Timer? autoSaveTimer;

>>>>>>> main
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
        BackButton = GetNode<BaseButton>("%BackButton");
        EndingContentContainer = GetNode<Container>("%EndingContentContainer");
        TutorialButton = GetNode<BaseButton>("%TutorialButton");
        TutorialCloseButton = GetNode<BaseButton>("%TutorialCloseButton");
        TutorialLayer = GetNode<Control>("%Tutorial");

        // connect signals
        ShowUpgradesButton.Pressed += onUpgradesButtonPressed;
        SignalBus.Instance.RainbowPlaced += onRainbowPlaced;
        BackButton.Pressed += onBackButtonPressed;
        TutorialButton.Pressed += onTutorialButtonPressed;
        TutorialCloseButton.Pressed += TutorialLayer.Hide;

        // setup
        GameLayer.Show();
        EndLayer.Hide();
        GameLayer.GetNode<Camera>("Camera").MakeCurrent();

        GameStore.LoadGame();

        AddChild(new Autosave());

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

    private async Task LoadPlayerSaveAsync()
    {
        var firebaseAuth = GetNode<FirebaseAuth>("/root/FirebaseAuth");
        var firebaseSave = GetNode<FirebaseSave>("/root/FirebaseSave");

        if (!string.IsNullOrWhiteSpace(firebaseAuth.RefreshToken))
        {
            var remoteSave = await firebaseSave.LoadAsync(firebaseAuth);
            if (remoteSave != null)
            {
                GameStore.LoadFromSaveData(remoteSave);
                return;
            }
        }

        GameStore.LoadGame();
    }

    private void onTutorialButtonPressed()
    {
        Services.Get<AudioSystem>().PlaySound("click");
        if (TutorialLayer.Visible)
        {
            TutorialLayer.Hide();
        }
        else
        {
            TutorialLayer.Show();
        }
    }

    private void onUpgradesButtonPressed()
    {
        Services.Get<AudioSystem>().PlaySound("click");
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

    public override async void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("save_game"))
        {
<<<<<<< HEAD
            await GameStore.SaveGameAsync();
            GD.Print("Saved Game!");
=======
            GameStore.SaveGame();
>>>>>>> main
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

        GameEndJob.SpeedScale = 1.0f;
        Tween tween = CreateTween();
        tween.TweenMethod(
            Callable.From((float v) => GameEndJob.SpeedScale = v),
            GameEndJob.SpeedScale,
            GameStore.RainbowSpeedScaleMax,
            GameStore.GameEndAnimationTime
        );
        tween.SetEase(Tween.EaseType.In);
        tween.SetTrans(Tween.TransitionType.Expo);
        GameStore.GameEnd = true;

        UILayer.Hide();

        Tween cameraTween = CreateTween();
        cameraTween.TweenMethod(
            Callable.From((float v) => camera.SetZoom(v)),
            GameStore.GameEndStartZoom,
            GameStore.GameEndEndZoom,
            GameStore.GameEndAnimationTime
        );

        ColorRect whiteScreen = GetNode<ColorRect>("%WhiteScreen");
        whiteScreen.Modulate = new Color(1, 1, 1, 0);
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

    private void onBackButtonPressed()
    {
        Services.Get<AudioSystem>().PlaySound("click");
        EndLayer.Hide();
        EndingContentContainer.Modulate = new Color(1, 1, 1, 0);
        GameLayer.Show();
        PlacementMenu.Hide();
        UILayer.Show();
        CollapseButton.Show();
        Services.Get<PlacementSystem>().ClearHoverText();
        GameLayer.GetNode<Camera>("Camera").MakeCurrent();
        GameLayer.GetNode<Camera>("Camera").ControlsEnabled = true;
    }
}
