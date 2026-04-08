public abstract class UnitState
{
    protected UnitStateController controller;

    public UnitState(UnitStateController controller)
    {
        this.controller = controller;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Tick() { }
}