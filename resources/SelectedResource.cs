using System;
using Godot;

/// A container for a resource and its image.
[GlobalClass]
public partial class SelectedResource
    : Resource,
        IHasHoverTitle,
        IHasHoverDescription,
        IHasHoverSubtitle,
        IHasHoverPrice,
        IHasHoverRefresh
{
    [Export]
    public Resource Resource;

    [Export]
    public Texture2D Icon;

    [Export]
    public string UnlockKey;

    public bool IsUnlocked() => string.IsNullOrEmpty(UnlockKey) || GameStore.IsUnlocked(UnlockKey);

    public virtual string GetHoverTitle()
    {
        if (Resource is PackedScene s && s.Instantiate() is IHasHoverTitle t)
            return t.GetHoverTitle();
        return "";
    }

    public virtual string GetHoverDescription()
    {
        if (Resource is PackedScene s && s.Instantiate() is IHasHoverDescription d)
            return d.GetHoverDescription();
        return "";
    }

    public virtual string GetHoverSubtitle()
    {
        if (Resource is PackedScene s && s.Instantiate() is IHasHoverSubtitle u)
            return u.GetHoverSubtitle();
        return "";
    }

    public virtual int GetHoverCost()
    {
        if (Resource is PackedScene s && s.Instantiate() is IHasHoverPrice p)
            return p.GetHoverCost();
        return 0;
    }

    public virtual bool IsEnough()
    {
        if (Resource is PackedScene s && s.Instantiate() is IHasHoverPrice p)
            return p.IsEnough();
        return false;
    }

    public virtual void RegisterRefresh(Action onRefresh)
    {
        if (Resource is PackedScene s && s.Instantiate() is IHasHoverRefresh r)
            r.RegisterRefresh(onRefresh);
    }

    public virtual void UnregisterRefresh(Action onRefresh)
    {
        if (Resource is PackedScene s && s.Instantiate() is IHasHoverRefresh r)
            r.UnregisterRefresh(onRefresh);
    }
}
