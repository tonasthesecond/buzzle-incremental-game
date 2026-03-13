using Godot;

public partial class Utils : RefCounted
{
    /// Smoothly interpolates between two floats. Framerate-independent.
    /// - smoothness: The higher the smoother/slower the transition.
    public static float SmoothedLerp(float from, float to, float delta, float smoothness = 5f)
    {
        return Mathf.Lerp(from, to, 1f - Mathf.Exp(-delta * smoothness));
    }

    /// Smoothly interpolates between two vectors. Framerate-independent.
    /// - smoothness: The higher the smoother/slower the transition.
    public static Vector2 SmoothedLerp(Vector2 from, Vector2 to, float delta, float smoothness = 5f)
    {
        return from.Lerp(to, 1f - Mathf.Exp(-delta * smoothness));
    }

    public static T GetRandom<T>(T[] array) => array[GD.Randi() % array.Length];
}
