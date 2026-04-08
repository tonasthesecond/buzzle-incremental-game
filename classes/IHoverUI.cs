using Godot;

public interface IHoverUI
{
    void Setup(Node target);
}

public interface IHasHoverDescription
{
    string GetHoverDescription();
}
