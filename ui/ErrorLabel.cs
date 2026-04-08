using Godot;

public partial class ErrorLabel : Label
{
    [Export]
    private Vector2 offset = new(0, -16);

    private const float fadeOutTime = 2f;
    private Tween tween;

    public override void _EnterTree()
    {
        Text = "";
        Visible = false;
        Modulate = Colors.Red;
        Services.Register(this);
    }

    public override void _Process(double delta)
    {
        GlobalPosition = GetGlobalMousePosition() + offset + new Vector2(-GetRect().Size.X / 2, 0);
    }

    /// Shows an error message.
    public void ShowError(FailMessage message)
    {
        Text = message.GameMessage;
        if (Text == "")
            return;
        Visible = true;
        Modulate = Colors.Red;
        TweenFadeOut();
    }

    /// Fades out the error message.
    private void TweenFadeOut()
    {
        if (tween != null)
            tween.Kill();
        tween = GetTree().CreateTween();
        tween.TweenProperty(this, "modulate:a", 0, fadeOutTime);
        SetProcess(true);

        void onTweenFinished()
        {
            tween.Kill();
            SetProcess(false);
            tween.Finished -= onTweenFinished;
        }
        tween.Finished += onTweenFinished;
    }
}
