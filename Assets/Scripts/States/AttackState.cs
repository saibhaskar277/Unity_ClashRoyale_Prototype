using UnityEngine;

public class AttackState : UnitState
{
    public AttackState(UnitStateController controller) : base(controller) { }

    public override void Enter()
    {
        controller.StopMovement();
    }

    public override void Tick()
    {
        if (controller.CurrentTarget == null || controller.CurrentTarget.Target == null || !controller.CurrentTarget.Target.gameObject.activeInHierarchy)
        {
            controller.ChangeState(controller.IdleState);
            controller.CurrentTarget = null;
            return;
        }

        controller.FaceTowards(controller.CurrentTarget.Target.position);

        float dist = controller.GetCombatDistance(controller.CurrentTarget.Target.position);

        // Target moved away
        if (dist > controller.AttackRange)
        {
            controller.ChangeState(controller.MoveState);
            return;
        }

        // Attack only (no movement / no retarget)
        if (controller.AttackSystem.CanAttack())
        {
            if (controller.AbilityManager != null && controller.AbilityManager.HasAbility)
            {
                controller.AbilityManager.TryUse(controller, controller.CurrentTarget);
            }
            else
            {
                controller.AttackSystem.Attack(controller.CurrentTarget);
            }
        }
    }
}