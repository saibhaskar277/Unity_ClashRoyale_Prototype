using UnityEngine;
using UnityEngine.AI;

public class UnitStateController : MonoBehaviour , IUnit 
{
    public NavMeshAgent Agent { get; private set; }
    public IAttack AttackSystem { get; private set; }
    public ITargetingStrategy TargetingStrategy { get; private set; }

    public IDamageable CurrentTarget;

    [Header("Config")]
    public float DetectionRadius = 10f;

    public float AttackRange = 2f;
    public LayerMask EnemyMask;

    UnitState currentState;

    public bool IsTargetingTower { get; set; }

    // States
    public IdleState IdleState { get; private set; }
    public MoveState MoveState { get; private set; }
    public AttackState AttackState { get; private set; }

    public UnitTeam Type => unitType;

    public TargetAttackType TargetAttackType { get; set; }


    [SerializeField] UnitTeam unitType;
    bool hasInitializedFromSpawnData;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        AttackSystem = GetComponent<IAttack>();
        TargetingStrategy = GetComponent<ITargetingStrategy>();
        TargetAttackType = AttackSystem != null ? AttackSystem.AttackType : TargetAttackType.Both;

        IdleState = new IdleState(this);
        MoveState = new MoveState(this);
        AttackState = new AttackState(this);
    }

    private void Start()
    {
        // Fallback for pre-placed scene units that are not spawned through a system.
        if (!hasInitializedFromSpawnData)
        {
            SetUnitData(new UnitSpawnData
            {
                currentUnitType = unitType,
                unitData = null
            });
        }
    }

    public void SetUnitData(UnitSpawnData data)
    {
        if (data == null)
            return;

        hasInitializedFromSpawnData = true;
        unitType = data.currentUnitType;

        if (data.unitData != null)
        {
            AttackRange = data.unitData.AttackRange;
            DetectionRadius = data.unitData.DetectionRadius;
            TargetAttackType = data.unitData.AttackType;
            ApplyRuntimeUnitData(data.unitData);
        }

        CurrentTarget = null;
        IsTargetingTower = true;
        if (Agent != null)
        {
            Agent.isStopped = false;
            Agent.ResetPath();
        }

        if (unitType != UnitTeam.PlayerTeam)
        {
            EnemyMask = LayerMask.GetMask("TeamOne");
            gameObject.layer = LayerMask.NameToLayer("TeamTwo");
        }
        else
        {
            EnemyMask = LayerMask.GetMask("TeamTwo");
            gameObject.layer = LayerMask.NameToLayer("TeamOne");
        }

        ChangeState(IdleState);

    }

    void ApplyRuntimeUnitData(UnitData data)
    {
        if (Agent != null)
        {
            Agent.speed = data.MoveSpeed;
        }

        var attacking = GetComponent<AttackingSystem>();
        if (attacking != null)
        {
            attacking.Configure(data);
        }

        var health = GetComponent<HealthManager>();
        if (health != null)
        {
            health.Configure(data);
        }
    }

    private void Update()
    {
        currentState?.Tick();
    }

    public void ChangeState(UnitState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    // =====================
    // TARGETING
    // =====================

    public IDamageable FindNearestTarget()
    {
        return TargetingStrategy?.GetTarget(transform, DetectionRadius, EnemyMask, TargetAttackType);
    }


    public IDamageable FindNearestTower()
    {
        return TargetingStrategy?.GetTower(transform, 50f, EnemyMask);
    }

}