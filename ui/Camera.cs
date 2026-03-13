using Godot;

public partial class Camera : Camera2D
{
    [Export] public float PanSpeed = 1.0f;
    [Export] public float ZoomSpeed = 0.3f;
    [Export] public float MinZoom = 0.3f;
    [Export] public float MaxZoom = 2.0f;
    [Export] public bool UseSmoothing = true;
    [Export] public float SmoothSpeed = 10.0f;
    [Export] public Vector2 CenterPosition = Vector2.Zero; // Where "center" is

    private Vector2 _targetPosition;
    private Vector2 _targetZoom;
    private bool _isDragging = false;
    private Vector2 _dragStartMousePos;
    private Vector2 _dragStartCameraPos;

    public override void _Ready()
    {
        _targetPosition = Position;
        _targetZoom = Zoom;
        
        // Make this the active camera
        MakeCurrent();
    }

    public override void _Process(double delta)
    {
        if (UseSmoothing)
        {
            // Smoothly interpolate position and zoom
            Position = Position.Lerp(_targetPosition, (float)delta * SmoothSpeed);
            Zoom = Zoom.Lerp(_targetZoom, (float)delta * SmoothSpeed);
        }
        else
        {
            Position = _targetPosition;
            Zoom = _targetZoom;
        }
    }

    public override void _Input(InputEvent @event)
    {
        // Keyboard input
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (keyEvent.Keycode == Key.E)
            {
                ResetToCenter();
            }
        }

        // Mouse wheel zoom
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.WheelUp)
            {
                ZoomAtCenter(Zoom * (1 + ZoomSpeed));
            }
            else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
            {
                ZoomAtCenter(Zoom * (1 - ZoomSpeed));
            }
            // Start dragging with middle mouse or right click
            else if (mouseButton.ButtonIndex == MouseButton.Middle ||
                     mouseButton.ButtonIndex == MouseButton.Right)
            {
                if (mouseButton.Pressed)
                {
                    _isDragging = true;
                    _dragStartMousePos = mouseButton.Position;
                    _dragStartCameraPos = _targetPosition;
                }
                else
                {
                    _isDragging = false;
                }
            }
        }
        
        // Pan/drag with mouse
        if (@event is InputEventMouseMotion mouseMotion && _isDragging)
        {
            Vector2 dragDelta = (mouseMotion.Position - _dragStartMousePos) / Zoom;
            _targetPosition = _dragStartCameraPos - dragDelta;
        }
    }

    private void ZoomAtCenter(Vector2 newZoom)
    {
        // Clamp zoom between min and max
        newZoom = newZoom.Clamp(new Vector2(MinZoom, MinZoom), new Vector2(MaxZoom, MaxZoom));
        
        // Zoom toward center of screen
        _targetZoom = newZoom;
    }

    private void ResetToCenter()
    {
        _targetPosition = new Vector2(320, 241); //center of the game
        _targetZoom = new Vector2(1, 1); // Reset zoom to default
    }
}