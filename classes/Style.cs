using System;
using System.Text;
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

    public static string CKPercent(float value, string colorKey = "primary_color")
    {
        return ColorKey((value * 100f).ToString("F0"), colorKey) + "%";
    }

    /// --- Number Change ---
    // format float without trailing .0 if whole
    private static string FormatFloat(float value, int roundTo)
    {
        float rounded = MathF.Round(value, roundTo);
        return rounded == MathF.Floor(rounded)
            ? ((int)rounded).ToString()
            : rounded.ToString($"F{roundTo}");
    }

    private static string NumberChange(string originalValue, string newValue) =>
        $"[color={GameStore.Colors["number_original"]}]{originalValue}[/color] ➜ [color={GameStore.Colors["number_new"]}]{newValue}[/color]";

    private static string NumberNew(string value) =>
        $"[color={GameStore.Colors["number_new"]}]{value}[/color]";

    public static string NumberChange(int originalValue, int newValue, bool showChange = true) =>
        showChange
            ? NumberChange(originalValue.ToString(), newValue.ToString())
            : NumberNew(originalValue.ToString());

    public static string NumberChange(
        float originalValue,
        float newValue,
        int roundTo = 1,
        bool showChange = true
    ) =>
        showChange
            ? NumberChange(FormatFloat(originalValue, roundTo), FormatFloat(newValue, roundTo))
            : NumberNew(FormatFloat(originalValue, roundTo));

    public static string NumberChangePercent(
        float originalValue,
        float newValue,
        bool showChange = true
    ) =>
        showChange
            ? NumberChange(
                MathF.Round(originalValue * 100).ToString("F0"),
                MathF.Round(newValue * 100).ToString("F0")
            ) + "%"
            : NumberNew(MathF.Round(originalValue * 100).ToString("F0")) + "%";

    public static string NC(
        float originalValue,
        float newValue,
        int roundTo = 1,
        bool showChange = true
    ) => NumberChange(originalValue, newValue, roundTo, showChange);

    public static string NC(int originalValue, int newValue, bool showChange = true) =>
        NumberChange(originalValue, newValue, showChange);

    public static string NC(string originalValue, string newValue, bool showChange = true) =>
        showChange ? NumberChange(originalValue, newValue) : NumberNew(originalValue);

    public static string NCPercent(float originalValue, float newValue, bool showChange = true) =>
        NumberChangePercent(originalValue, newValue, showChange);

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

    // --- Flavor Text ---
    public static string ParseFlavorText(string flavorText)
    {
        // parse flavor text format of [color_id](text) into Style.CK(text, color_id)
        StringBuilder res = new StringBuilder();
        int i = 0;

        while (i < flavorText.Length)
        {
            if (flavorText[i] == '[')
            {
                int closeBracket = flavorText.IndexOf(']', i);
                int openParen = closeBracket + 1;
                int closeParen = flavorText.IndexOf(')', openParen);

                string colorId = flavorText.Substring(i + 1, closeBracket - i - 1);
                string text = flavorText.Substring(openParen + 1, closeParen - openParen - 1);

                res.Append(CK(text, colorId));
                i = closeParen + 1;
            }
            else
            {
                res.Append(flavorText[i]);
                i++;
            }
        }

        return res.ToString();
    }
}
