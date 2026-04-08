using Godot;

[GlobalClass]
public partial class BaseTile : Node2D
{
    public Vector2I GridPosition { get; set; }

    private void ModifyObject(BaseGridObject obj)
    {
        switch (obj)
        {
            case HiveGridObject hive:
                ModifyHive(hive);
                break;
            case Flower flower:
                ModifyFlower(flower);
                break;
        }
    }

    protected virtual void ModifyHive(HiveGridObject hive) { }

    protected virtual void ModifyFlower(Flower flower) { }

    public override void _Ready()
    {
        SignalBus.Instance.GridObjectPlaced += ModifyObject;
        SignalBus.Instance.GameLoaded += () =>
        {
            BaseGridObject? obj = Services.Get<Grid>().GetObjectAt(GridPosition);
            if (obj != null)
                ModifyObject(obj);
        };
    }
}
