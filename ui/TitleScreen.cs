using Godot;

public partial class TitleScreen : Node
{
    private Control title = null!;
    private Control bee = null!;
    private BaseButton loginButton = null!;
    private TextEdit usernameField = null!;
    private TextEdit passwordField = null!;

    private float _time = 0f;
    private float _titleBaseY;
    private float _beeBaseY;
    private float _beeX = 32f;
    private int _beeDir = 1;

    private const float BeeSpeed = 120f;
    private const float TitleAmplitude = 5f;
    private const float TitleFrequency = 1.4f;
    private const float BeeAmplitude = 16f;
    private const float BeeFrequency = 4f;

    public override void _Ready()
    {
        title = GetNode<Control>("%Title");
        bee = GetNode<Control>("%Bee");
        loginButton = GetNode<BaseButton>("%LoginButton");
        usernameField = GetNode<TextEdit>("%UsernameField");
        passwordField = GetNode<TextEdit>("%PasswordField");

        _titleBaseY = title.Position.Y;
        _beeBaseY = bee.Position.Y;
        bee.Position = new Vector2(_beeX, _beeBaseY);

        loginButton.Pressed += OnLoginButtonPressed;
    }

    public override void _Process(double delta)
    {
        _time += (float)delta;
        float dt = (float)delta;

        float titleY = _titleBaseY + Mathf.Sin(_time * TitleFrequency) * TitleAmplitude;
        title.Position = new Vector2(title.Position.X, titleY);

        float beeY = _beeBaseY + Mathf.Sin(_time * BeeFrequency + 0.8f) * BeeAmplitude;

        float screenWidth = GetViewport().GetVisibleRect().Size.X;
        float minX = 32f;
        float maxX = screenWidth - bee.GetRect().Size.X - 32f;

        _beeX += BeeSpeed * _beeDir * dt;

        if (_beeX >= maxX)
        {
            _beeX = maxX;
            _beeDir = -1;
            bee.ZIndex = GD.Randi() % 2 == 0 ? -1 : 1;
            bee.Scale = new Vector2(-1f, 1f);
        }
        else if (_beeX <= minX)
        {
            _beeX = minX;
            _beeDir = 1;
            bee.ZIndex = GD.Randi() % 2 == 0 ? -1 : 1;
            bee.Scale = new Vector2(1f, 1f);
        }

        bee.Position = new Vector2(_beeX, beeY);
    }

    private void OnLoginButtonPressed()
    {
        string username = usernameField.Text;
        string password = passwordField.Text;
        // TODO: Authenticate here

        GetTree().ChangeSceneToFile("res://Game.tscn");
    }
}
