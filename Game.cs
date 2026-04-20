using Godot;

public partial class Game : GameSystem
{
    public Node2D GameLayer { get; private set; } = null!;
    public Node2D UpgradeLayer { get; private set; } = null!;
    public CanvasLayer UILayer { get; private set; } = null!;
    public BaseButton ShowUpgradesButton { get; private set; } = null!;
    public Control PlacementMenu { get; private set; } = null!;
    public Control CollapseButton { get; private set; } = null!;

    public override void _Ready()
    {
        // get nodes
        GameLayer = GetNode<Node2D>("%GameLayer");
        UpgradeLayer = GetNode<Node2D>("%UpgradeLayer");
        UILayer = GetNode<CanvasLayer>("%UILayer");
        ShowUpgradesButton = GetNode<BaseButton>("%ShowUpgradesButton");
        PlacementMenu = GetNode<Control>("%PlacementMenu");
        CollapseButton = UILayer.GetNode<Control>("CollapseButton");

        // connect signals
        ShowUpgradesButton.Pressed += onUpgradesButtonPressed;

        // setup
        GameLayer.Show();
        GameLayer.GetNode<Camera>("Camera").MakeCurrent();

        // load game
        GameStore.LoadGame();
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
    }
}
