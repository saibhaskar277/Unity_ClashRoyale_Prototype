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

        controller.Agent.isStopped = false;
        controller.Agent.SetDestination(controller.CurrentTarget.Target.position);
    }

    public override void Tick()
    {
        if (controller.CurrentTarget == null || controller.CurrentTarget.Target == null || !controller.CurrentTarget.Target.gameObject.activeInHierarchy)
        {
            controller.CurrentTarget = null;
            controller.ChangeState(controller.IdleState);
            return;
        }
        checkTimer.Tick();
        if (controller.IsTargetingTower && checkTimer.IsFinished)
        {
            var target = controller.FindNearestTarget();
            if (target != null)
            {
                controller.CurrentTarget = target;
                controller.Agent.SetDestination(controller.CurrentTarget.Target.position);
                controller.IsTargetingTower = false;
            }
            checkTimer.Reset();
        }



        float dist = Vector3.Distance(controller.transform.position, controller.CurrentTarget.Target.position);

        // Enter attack
        if (dist <= controller.AttackRange)
        {
            controller.ChangeState(controller.AttackState);
            return;
        }

        // Retarget while moving
        if (!controller.AttackSystem.CanAttack())
            return;

        IDamageable newTarget = controller.FindNearestTarget();

        if (newTarget != null && newTarget != controller.CurrentTarget)
        {
            controller.CurrentTarget = newTarget;
            if (controller.CurrentTarget.Target != null)
            {
                controller.Agent.SetDestination(controller.CurrentTarget.Target.position);
            }
        }


    }
}