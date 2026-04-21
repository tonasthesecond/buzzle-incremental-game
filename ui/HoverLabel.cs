using Godot;

public partial class HoverLabel : Label
{
    [Export]
    private Vector2 offset = new(0, -16);
    private const float fadeOutTime = 1.5f;
    private Tween tween;
    private string persistentText = "";
    private Color persistentColor = Colors.White;
    private bool hasPersistent = false;

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

    /// Shows a persistent message following the mouse.
    public void ShowMessage(string message, Color color)
    {
        if (isError)
            return;
        tween?.Kill();
        persistentText = message;
        persistentColor = color;
        hasPersistent = true;
        Text = message;
        Visible = true;
        Modulate = color;
        SetProcess(true);
    }

    public void HideMessage()
    {
        hasPersistent = false;
        persistentText = "";
        Visible = false;
    }

    public bool isError = false;

    /// Shows a transient error message.
    public void ShowError(FailMessage message)
    {
        if (message.GameMessage == "")
            return;
        Text = message.GameMessage;
        Visible = true;
        Modulate = Colors.Red;
        isError = true;

        tween?.Kill();
        tween = GetTree().CreateTween();
        tween.TweenProperty(this, "modulate:a", 0, fadeOutTime);
        SetProcess(true);
        tween.Finished += () =>
        {
            isError = false;
            if (hasPersistent)
            {
                Text = persistentText;
                Modulate = persistentColor with { A = 0 };
                Visible = true;
                tween = GetTree().CreateTween();
                tween.TweenProperty(this, "modulate:a", 1, 0.3f);
            }
            else
            {
                Visible = false;
                SetProcess(false);
            }
        };
    }

    private void TweenFadeOut()
    {
        tween?.Kill();
        tween = GetTree().CreateTween();
        tween.TweenProperty(this, "modulate:a", 0, fadeOutTime);
        SetProcess(true);
        tween.Finished += () =>
        {
            Visible = false;
            SetProcess(false);
        };
    }
}
