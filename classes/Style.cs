using Godot;

public class Style
{
    /// --- Color Key ---
    public static string ColorKey(string name, string colorKey = "primary_color")
    {
        string color;
        if (!GameStore.Colors.ContainsKey(colorKey))
        {
            GD.PrintErr($"[Style] Missing color key: {colorKey}");
            color = GameStore.Colors["primary_color"];
        }
        else
            color = GameStore.Colors[colorKey];
        return $"[color={color}]{name}[/color]";
    }

    public static string CK(string name) => ColorKey(name);

    public static string CK(string name, string colorKey) => ColorKey(name, colorKey);

    public static string CK(float value, string colorKey = "primary_color") =>
        ColorKey(value.ToString("F0"), colorKey);

    public static string CKPercent(float value, string colorKey = "primary_color")
    {
        return ColorKey((value * 100f).ToString("F0"), colorKey) + "%";
    }

    /// --- Number Change ---
    private static string NumberChange(string originalValue, string newValue) =>
        $"[color={GameStore.Colors["number_original"]}]{originalValue}[/color] ➜ [color={GameStore.Colors["number_new"]}]{newValue}[/color]";

    public static string NumberChange(int originalValue, int newValue) =>
        NumberChange(originalValue.ToString(), newValue.ToString());

    public static string NumberChange(float originalValue, float newValue, int roundTo = 1)
    {
        string fmt = $"F{roundTo}";
        return NumberChange(originalValue.ToString(fmt), newValue.ToString(fmt));
    }

    public static string NumberChangePercent(float originalValue, float newValue)
    {
        return NumberChange(originalValue * 100, newValue * 100, 0) + "%";
    }

    public static string NC(float originalValue, float newValue, int roundTo = 1) =>
        NumberChange(originalValue, newValue, roundTo);

    public static string NC(int originalValue, int newValue) =>
        NumberChange(originalValue, newValue);

    public static string NCPercent(float originalValue, float newValue) =>
        NumberChangePercent(originalValue, newValue);

    /// --- Title ---
    public static string Title(string text) =>
        $"[b][color={GameStore.Colors["title"]}]{text}[/color][/b]";

    public static string SubTitle(string text) =>
        $"[color={GameStore.Colors["subtitle"]}]{text}[/color]";

    /// --- Price ---
    public static string Price(int price, bool isEnough = true)
    {
        string colorKey = isEnough ? "price_enough" : "price_not_enough";
        return $"[color={GameStore.Colors[colorKey]}]${price}[/color]";
    }
}
