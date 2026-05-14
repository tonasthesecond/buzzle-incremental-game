using Godot;

[GlobalClass]
public partial class BaseTile : Node2D, IHasHoverTitle, IHasHoverDescription, IHasHoverPrice
{
    public Vector2I GridPosition { get; set; }

    private void ModifyObject(BaseGridObject obj)
    {
        switch (obj)
        {
            case Hive hive:
                ModifyHive(hive);
                break;
            case Flower flower:
                ModifyFlower(flower);
                break;
        }
    }

    protected virtual void ModifyHive(Hive hive) { }

    protected virtual void ModifyFlower(Flower flower) { }

    public override void _Ready()
    {
        SignalBus.Instance.GridObjectPlaced += ModifyObject;
        SignalBus.Instance.GameLoaded += OnGameLoaded;
    }

    public override void _ExitTree()
    {
        if (!IsInstanceValid(SignalBus.Instance))
            return;
        SignalBus.Instance.GridObjectPlaced -= ModifyObject;
        SignalBus.Instance.GameLoaded -= OnGameLoaded;
    }

    private void OnGameLoaded()
    {
        BaseGridObject? obj = Services.Get<Grid>().GetObjectAt(GridPosition);
        if (obj != null)
            ModifyObject(obj);
    }

    public virtual string GetHoverTitle() => "<Tile>";

    public virtual string GetHoverDescription() => "<Tile>";

    public int GetHoverCost() => GameStore.GetPlacementCost(GetType());

    public bool IsEnough() => GameStore.Honey >= GetHoverCost();
}
