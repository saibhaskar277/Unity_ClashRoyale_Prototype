using UnityEngine;

public class MoveState : UnitState
{
    public MoveState(UnitStateController controller) : base(controller) { }

    float timerToCheck = 0.5f;
    Timer checkTimer;

    public override void Enter()
    {
        checkTimer = new Timer(timerToCheck);
        if (controller.CurrentTarget == null || controller.CurrentTarget.Target == null)
        {
            controller.ChangeState(controller.IdleState);
            return;
        }

        controller.BeginMovementTo(controller.CurrentTarget.Target.position);
    }

    public override void Tick()
    {
        if (controller.CurrentTarget == null || controller.CurrentTarget.Target == null || !controller.CurrentTarget.Target.gameObject.activeInHierarchy)
        {
            controller.CurrentTarget = null;
            controller.ChangeState(controller.IdleState);
            return;
        }

        controller.FaceTowards(controller.CurrentTarget.Target.position);
        checkTimer.Tick();
        if (controller.IsTargetingTower && checkTimer.IsFinished)
        {
            var target = controller.FindNearestTarget();
            if (target != null)
            {
                controller.CurrentTarget = target;
                controller.BeginMovementTo(controller.CurrentTarget.Target.position);
                controller.IsTargetingTower = false;
            }
            checkTimer.Reset();
        }



        float dist = controller.GetCombatDistance(controller.CurrentTarget.Target.position);

        // Enter attack
        if (dist <= controller.AttackRange)
        {
            controller.ChangeState(controller.AttackState);
            return;
        }

        controller.TickMovementTowards(controller.CurrentTarget.Target.position);

        // Retarget while moving
        if (!controller.AttackSystem.CanAttack())
            return;

        IDamageable newTarget = controller.FindNearestTarget();

        if (newTarget != null && newTarget != controller.CurrentTarget)
        {
            controller.CurrentTarget = newTarget;
            if (controller.CurrentTarget.Target != null)
            {
                controller.BeginMovementTo(controller.CurrentTarget.Target.position);
            }
        }


    }
}