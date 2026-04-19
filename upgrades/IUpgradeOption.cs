using System.Text;
using Godot;

[GlobalClass]
public abstract partial class IUpgradeOption : Resource, IHasHoverDescription
{
    [Signal]
    public delegate void AppliedEventHandler();

    [Export]
    public string Name { get; set; } = "Upgrade";

    [Export]
    public virtual int MaxLevel { get; set; } = -1; // -1 = infinite

    [Export]
    public IScaleModel CostScaler { get; set; }

    public int Level { get; set; } = 0; // How many times this upgrade has been bought

    [Export]
    public string FlavorText { get; set; } = "";

    public string GetHoverDescription()
    {
        // parse flavor text format of [color_id](text) into Style.CK(text, color_id)
        StringBuilder hoverDescription = new StringBuilder();
        string flavorText = FlavorText;
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

                hoverDescription.Append(Style.CK(text, colorId));
                i = closeParen + 1;
            }
            else
            {
                hoverDescription.Append(flavorText[i]);
                i++;
            }
        }

        if (GetTechnicalText() != "")
            hoverDescription.Append("\n\n" + GetTechnicalText());
        return hoverDescription.ToString();
    }

    public abstract string GetTechnicalText(); // text of what values have changed

    public abstract void Apply(); // apply upgrade

    public virtual int GetCost() => (int)CostScaler.Get(Level);

    public bool IsEnough() => GetCost() <= GameStore.Honey;

    // Check if this upgrade can be bought, and if not, return a message
    public virtual bool FailCondition(out FailMessage? fail_message)
    {
        fail_message = null;
        return false;
    }

    public bool Buy(out FailMessage? failMessage)
    {
        // check if enough honey and max level
        if (MaxLevel != -1 && Level >= MaxLevel)
        {
            failMessage = new FailMessage("Already at max level!");
            return false;
        }
        int cost = GetCost();
        if (GameStore.Honey < cost)
        {
            failMessage = new FailMessage("Not enough honey!");
            return false;
        }

        // check any other conditions
        if (FailCondition(out failMessage))
            return false;

        // buy upgrade
        GameStore.Honey -= cost;
        Level++;
        Apply();
        EmitSignal(SignalName.Applied);

        failMessage = null;
        return true;
    }
}
