using Godot;

[GlobalClass]
public partial class NumberDisplay : PanelContainer
{
    [Export]
    public Texture2D Icon { get; set; }

    private RichTextLabel numberLabel = null!;
    private TextureRect iconRect = null!;

    public override void _Ready()
    {
        numberLabel = GetNode<RichTextLabel>("%NumberLabel");
        iconRect = GetNode<TextureRect>("%IconRect");
        iconRect.Texture = Icon;
    }

    public void SetNumber(int number)
    {
        numberLabel.Text = number.ToString();
    }
}
