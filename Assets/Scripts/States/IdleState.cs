using UnityEngine;

public class IdleState : UnitState
{
    public IdleState(UnitStateController controller) : base(controller) { }

    public override void Enter()
    {
        controller.IsTargetingTower = true;
        if(controller.CurrentTarget == null)
        {
            controller.CurrentTarget = controller.FindNearestTower();
        }

        if (controller.CurrentTarget != null)
        {
            controller.ChangeState(controller.MoveState);
        }
    }
}