using UnityEngine;
using UnityEngine.AI;

public class UnitStateController : MonoBehaviour , IUnit 
{
    public NavMeshAgent Agent { get; private set; }
    public IAttack AttackSystem { get; private set; }
    public ITargetingStrategy TargetingStrategy { get; private set; }
    public UnitCategory MovementCategory { get; private set; } = UnitCategory.Grounded;
    public float MoveSpeed { get; private set; } = 3.5f;

    public IDamageable CurrentTarget;

    [Header("Config")]
    public float DetectionRadius = 10f;

    public float AttackRange = 2f;
    public float RotationSpeed = 540f;
    public LayerMask EnemyMask;
    [Header("Air Movement")]
    [SerializeField] bool useSpawnYAsAirHeight = true;
    [SerializeField] float airYHeight = 0f;

    UnitState currentState;
    float runtimeAirYHeight;

    public bool IsTargetingTower { get; set; }
    public bool UsesNavMeshMovement => MovementCategory == UnitCategory.Grounded && Agent != null && Agent.isOnNavMesh;

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

        ConfigureMovementComponentForUnitType();

        CurrentTarget = null;
        IsTargetingTower = true;
        runtimeAirYHeight = useSpawnYAsAirHeight ? transform.position.y : airYHeight;
        if (Agent != null && Agent.enabled)
        {
            Agent.isStopped = false;
            SafeResetPath();
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
        MoveSpeed = data.MoveSpeed;
        MovementCategory = data.UnitType;

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

    void ConfigureMovementComponentForUnitType()
    {
        if (Agent == null)
            return;

        bool shouldUseNavMesh = MovementCategory == UnitCategory.Grounded;
        if (Agent.enabled != shouldUseNavMesh)
        {
            Agent.enabled = shouldUseNavMesh;
        }

        if (shouldUseNavMesh)
        {
            Agent.isStopped = false;
            SafeResetPath();
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

    public void BeginMovementTo(Vector3 destination)
    {
        if (UsesNavMeshMovement)
        {
            Agent.isStopped = false;
            Agent.SetDestination(destination);
            return;
        }

        // Keep air units on a fixed Y (height) while moving on XZ.
        transform.position = new Vector3(transform.position.x, runtimeAirYHeight, transform.position.z);
    }

    public void TickMovementTowards(Vector3 destination)
    {
        if (UsesNavMeshMovement)
        {
            Agent.isStopped = false;
            Agent.SetDestination(destination);
            return;
        }

        Vector3 current = new Vector3(transform.position.x, runtimeAirYHeight, transform.position.z);
        Vector3 targetOnPlane = new Vector3(destination.x, runtimeAirYHeight, destination.z);
        transform.position = Vector3.MoveTowards(current, targetOnPlane, MoveSpeed * Time.deltaTime);
    }

    public void StopMovement()
    {
        if (UsesNavMeshMovement)
        {
            Agent.isStopped = true;
            SafeResetPath();
        }
    }

    void SafeResetPath()
    {
        if (Agent == null || !Agent.enabled || !Agent.isOnNavMesh)
            return;

        Agent.ResetPath();
    }

    public float GetCombatDistance(Vector3 targetPosition)
    {
        if (UsesNavMeshMovement)
            return Vector3.Distance(transform.position, targetPosition);

        // Air movement/combat should use XZ plane distance.
        Vector2 selfXZ = new Vector2(transform.position.x, transform.position.z);
        Vector2 targetXZ = new Vector2(targetPosition.x, targetPosition.z);
        return Vector2.Distance(selfXZ, targetXZ);
    }

    public void FaceTowards(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.0001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
    }

}