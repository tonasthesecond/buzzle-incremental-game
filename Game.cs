using Godot;

public partial class Game : GameSystem
{
    public Node2D GameLayer { get; private set; } = null!;
    public Node2D UpgradeLayer { get; private set; } = null!;
    public CanvasLayer UILayer { get; private set; } = null!;
    public BaseButton ShowUpgradesButton { get; private set; } = null!;

    public override void _Ready()
    {
        // get nodes
        GameLayer = GetNode<Node2D>("%GameLayer");
        UpgradeLayer = GetNode<Node2D>("%UpgradeLayer");
        UILayer = GetNode<CanvasLayer>("%UILayer");
        ShowUpgradesButton = GetNode<BaseButton>("%ShowUpgradesButton");

        // connect signals
        ShowUpgradesButton.Pressed += ShowUpgrades;

        // setup
        GameLayer.Show();
        GameLayer.GetNode<Camera>("Camera").MakeCurrent();
    }

    private void ShowUpgrades()
    {
        if (UpgradeLayer.Visible)
        {
            UpgradeLayer.Hide();
            GameLayer.Show();
            GameLayer.GetNode<Camera>("Camera").MakeCurrent();
        }
        else
        {
            UpgradeLayer.Show();
            GameLayer.Hide();
            UpgradeLayer.GetNode<Camera>("Camera").MakeCurrent();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("save_game"))
        {
            GD.Print("Saving game");
            GameStore.SaveGame();
        }
    }
}
