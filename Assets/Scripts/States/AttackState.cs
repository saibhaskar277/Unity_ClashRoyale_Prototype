using UnityEngine;

public class AttackState : UnitState
{
    public AttackState(UnitStateController controller) : base(controller) { }

    public override void Enter()
    {
        controller.Agent.isStopped = true;
    }

    public override void Tick()
    {
        if (controller.CurrentTarget == null || controller.CurrentTarget.Target == null || !controller.CurrentTarget.Target.gameObject.activeInHierarchy)
        {
            controller.ChangeState(controller.IdleState);
            controller.CurrentTarget = null;
            return;
        }

        float dist = Vector3.Distance(controller.transform.position, controller.CurrentTarget.Target.position);

        // Target moved away
        if (dist > controller.AttackRange)
        {
            controller.ChangeState(controller.MoveState);
            return;
        }

        // Attack only (no movement / no retarget)
        if (controller.AttackSystem.CanAttack())
        {
            if (controller.CurrentTarget.Target != null)
            {
                controller.AttackSystem.Attack(controller.CurrentTarget);
            }
        }
    }
}