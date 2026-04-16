using Godot;

[GlobalClass]
public partial class PlacementMenu : Control
{
    [Export]
    public float TweenDuration { get; set; } = 0.3f;

    [Export]
    public float PanelDropDistance { get; set; } = 100f;

    [Export]
    public float ButtonDropDistance { get; set; } = -40f;

    public BaseButton collapseButton = null!;
    private bool isCollapsed = false;
    private float panelOriginY;
    private float buttonOriginY;

    public override void _Ready()
    {
        collapseButton = GetParent().GetNode<BaseButton>("CollapseButton");
        collapseButton.Pressed += Toggle;
        Callable
            .From(() =>
            {
                panelOriginY = Position.Y;
                buttonOriginY = collapseButton.Position.Y;
            })
            .CallDeferred();
    }

    public void Toggle()
    {
        if (isCollapsed)
            Expand();
        else
            Collapse();
    }

    public void Collapse()
    {
        var panelTween = CreateTween();
        panelTween.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Back);
        panelTween.TweenProperty(
            this,
            "position:y",
            panelOriginY + PanelDropDistance,
            TweenDuration
        );

        var buttonTween = CreateTween();
        buttonTween.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Back);
        buttonTween.TweenProperty(
            collapseButton,
            "position:y",
            buttonOriginY + ButtonDropDistance,
            TweenDuration
        );

        isCollapsed = true;
    }

    public void Expand()
    {
        var panelTween = CreateTween();
        panelTween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        panelTween.TweenProperty(this, "position:y", panelOriginY, TweenDuration);

        var buttonTween = CreateTween();
        buttonTween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        buttonTween.TweenProperty(collapseButton, "position:y", buttonOriginY, TweenDuration);

        isCollapsed = false;
    }
}
