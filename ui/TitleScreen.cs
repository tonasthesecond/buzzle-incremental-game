using Godot;
using System.Threading.Tasks;

public partial class TitleScreen : Node
{
    private Control title = null!;
    private Control tile1 = null!;
    private Control tile2 = null!;
    private Control tile3 = null!;
    private Control bee = null!;
    private BaseButton loginButton = null!;
    private BaseButton registerButton = null!;
    private TextEdit usernameField = null!;
    private TextEdit passwordField = null!;
    private float _time = 0f;
    private float _titleBaseY;
    private float _tile1BaseY;
    private float _tile2BaseY;
    private float _tile3BaseY;
    private float _beeBaseY;
    private float _beeX = 32f;
    private int _beeDir = 1;

    private const float BeeSpeed = 120f;
    private const float TitleAmplitude = 5f;
    private const float TitleFrequency = 1.4f;
    private const float TileAmplitude = 10f;
    private const float TileFrequency = 2f;
    private const float BeeAmplitude = 16f;
    private const float BeeFrequency = 4f;

    // staggered phase offsets for wave effect across tiles
    private static readonly float[] TilePhaseOffsets = { 0f, 0.4f, 0.8f };

    private static readonly int[] BeeZOptions = { 0, 2, 4 };

    public override void _Ready()
    {
        title = GetNode<Control>("%Title");
        tile1 = GetNode<Control>("%Tile1");
        tile2 = GetNode<Control>("%Tile2");
        tile3 = GetNode<Control>("%Tile3");
        bee = GetNode<Control>("%Bee");
        loginButton = GetNode<BaseButton>("%LoginButton");
        registerButton = GetNode<BaseButton>("%RegisterButton");
        usernameField = GetNode<TextEdit>("%UsernameField");
        passwordField = GetNode<TextEdit>("%PasswordField");

        _titleBaseY = title.Position.Y;
        _tile1BaseY = tile1.Position.Y;
        _tile2BaseY = tile2.Position.Y;
        _tile3BaseY = tile3.Position.Y;
        _beeBaseY = bee.Position.Y;
        bee.Position = new Vector2(_beeX, _beeBaseY);

        loginButton.Pressed += OnLoginButtonPressed;
        registerButton.Pressed += OnRegisterButtonPressed;
    }

    public override void _Process(double delta)
    {
        _time += (float)delta;
        float dt = (float)delta;

        // title single bob
        title.Position = new Vector2(
            title.Position.X,
            _titleBaseY + Mathf.Sin(_time * TitleFrequency) * TitleAmplitude
        );

        // tile wave
        tile1.Position = new Vector2(
            tile1.Position.X,
            _tile1BaseY + Mathf.Sin(_time * TileFrequency + TilePhaseOffsets[0]) * TileAmplitude
        );
        tile2.Position = new Vector2(
            tile2.Position.X,
            _tile2BaseY + Mathf.Sin(_time * TileFrequency + TilePhaseOffsets[1]) * TileAmplitude
        );
        tile3.Position = new Vector2(
            tile3.Position.X,
            _tile3BaseY + Mathf.Sin(_time * TileFrequency + TilePhaseOffsets[2]) * TileAmplitude
        );

        // bee bounce
        float beeY = _beeBaseY + Mathf.Sin(_time * BeeFrequency + 0.8f) * BeeAmplitude;
        float screenWidth = GetViewport().GetVisibleRect().Size.X;
        float minX = 32f;
        float maxX = screenWidth - bee.GetRect().Size.X - 32f;

        _beeX += BeeSpeed * _beeDir * dt;

        if (_beeX >= maxX)
        {
            _beeX = maxX;
            _beeDir = -1;
            bee.ZIndex = BeeZOptions[GD.Randi() % BeeZOptions.Length];
            bee.Scale = new Vector2(-1f, 1f);
        }
        else if (_beeX <= minX)
        {
            _beeX = minX;
            _beeDir = 1;
            bee.ZIndex = BeeZOptions[GD.Randi() % BeeZOptions.Length];
            bee.Scale = new Vector2(1f, 1f);
        }

        bee.Position = new Vector2(_beeX, beeY);
    }

    private async void OnLoginButtonPressed()
    {
        if (!TryGetCredentials(out var email, out var password))
            return;

        SetAuthButtonsDisabled(true);
        try
        {
            if (!await TryLoginAsync(email, password))
                return;

            var firebaseAuth = GetNode<FirebaseAuth>("/root/FirebaseAuth");
            var firebaseSave = GetNode<FirebaseSave>("/root/FirebaseSave");
            var remoteSave = await firebaseSave.LoadAsync(firebaseAuth);

            if (remoteSave == null)
            {
                GD.PrintErr("[TitleScreen] Could not load save from Firebase. Aborting scene change.");
                return;
            }

            GameStore.WriteLocalSave(remoteSave);
            GetTree().ChangeSceneToFile("res://Game.tscn");
        }
        finally
        {
            SetAuthButtonsDisabled(false);
        }
    }

    private async void OnRegisterButtonPressed()
    {
        if (!TryGetCredentials(out var email, out var password))
            return;

        SetAuthButtonsDisabled(true);
        var registerSucceeded = await TryRegisterAsync(email, password);
        SetAuthButtonsDisabled(false);

        if (!registerSucceeded)
            return;

        GameStore.WriteLocalSave(new SaveData());
        GetTree().ChangeSceneToFile("res://Game.tscn");
    }

    private async Task<bool> TryLoginAsync(string email, string password)
    {
        var firebaseAuth = GetNode<FirebaseAuth>("/root/FirebaseAuth");
        var loggedIn = await firebaseAuth.LoginAsync(email, password);

        if (!loggedIn)
            GD.PrintErr("[TitleScreen] Firebase login failed. Check credentials and Firebase config.");

        return loggedIn;
    }

    private async Task<bool> TryRegisterAsync(string email, string password)
    {
        var firebaseAuth = GetNode<FirebaseAuth>("/root/FirebaseAuth");
        var registered = await firebaseAuth.RegisterAsync(email, password);

        if (!registered)
            GD.PrintErr("[TitleScreen] Firebase register failed. Check credentials and Firebase config.");

        return registered;
    }

    private const string EmailDomain = "@buzzle.local";

    private bool TryGetCredentials(out string email, out string password)
    {
        var username = usernameField.Text.Trim();
        password = passwordField.Text.Trim();
        email = username + EmailDomain;

        if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            return true;

        GD.PrintErr("[TitleScreen] Username and password are required.");
        return false;
    }

    private void SetAuthButtonsDisabled(bool disabled)
    {
        loginButton.Disabled = disabled;
        registerButton.Disabled = disabled;
    }
}
