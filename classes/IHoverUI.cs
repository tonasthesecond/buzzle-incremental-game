using System;

public interface IHasHoverTitle
{
    string GetHoverTitle();
}

public interface IHasHoverDescription
{
    string GetHoverDescription();
}

public interface IHasHoverSubtitle
{
    string GetHoverSubtitle();
}

public interface IHasHoverPrice
{
    int GetHoverCost();
    bool IsEnough();
}

public interface IHasHoverRefresh
{
    void RegisterRefresh(Action onRefresh);
    void UnregisterRefresh(Action onRefresh);
}
