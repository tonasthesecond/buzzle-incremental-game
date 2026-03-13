using Godot;

public partial class Camera : Camera2D
{
    [Export] public float PanSpeed = 1.0f;
    [Export] public float ZoomSpeed = 0.1f;
    [Export] public float MinZoom = 0.5f;
    [Export] public float MaxZoom = 2.0f;
    [Export] public bool UseSmoothing = true;
    [Export] public float SmoothSpeed = 10.0f;

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
        // Mouse wheel zoom
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.WheelUp)
            {
                ZoomAtPoint(Zoom * (1 + ZoomSpeed), mouseButton.Position);
            }
            else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
            {
                ZoomAtPoint(Zoom * (1 - ZoomSpeed), mouseButton.Position);
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

    private void ZoomAtPoint(Vector2 newZoom, Vector2 point)
    {
        // Clamp zoom
        newZoom = newZoom.Clamp(new Vector2(MinZoom, MinZoom), new Vector2(MaxZoom, MaxZoom));
        
        // Zoom towards mouse position
        Vector2 zoomCenter = point - GetViewportRect().Size / 2;
        Vector2 ratio = (newZoom / Zoom) - new Vector2(1, 1);
        _targetPosition += zoomCenter * ratio / newZoom;
        _targetZoom = newZoom;
    }
}