using UnityEngine;

public class Tower : MonoBehaviour
{
    IAttack attackSystem;
    AttackingSystem attackingSystemComponent;
    HealthManager healthManager;

    Transform currentTarget;

    [Header("Config")]
    [SerializeField] float detectionRadius = 10f;
    [SerializeField] LayerMask targetLayer;

    [SerializeField] float attackRange = 8f;
    [SerializeField] TowerData towerData;
    [SerializeField] UnitTeam towerTeam = UnitTeam.PlayerTeam;
    [SerializeField] bool isMainTower;

    public bool canAttack = true;
    public bool CanAttack => canAttack;
    public UnitTeam Team => towerTeam;
    public bool IsMainTower => isMainTower;
    bool hasRaisedDestroyedEvent;


    private void Awake()
    {
        attackSystem = GetComponent<IAttack>();
        attackingSystemComponent = GetComponent<AttackingSystem>();
        healthManager = GetComponent<HealthManager>();

        ApplyData();
    }

    void OnEnable()
    {
        hasRaisedDestroyedEvent = false;
    }

    void OnDisable()
    {
        if (hasRaisedDestroyedEvent || !gameObject.scene.isLoaded)
            return;

        hasRaisedDestroyedEvent = true;
        EventManager.RaiseEvent(new TowerDestroyedEvent
        {
            Tower = this,
            Team = Team,
            IsMainTower = IsMainTower
        });
    }

    void ApplyData()
    {
        if (towerData == null)
            return;

        detectionRadius = towerData.DetectionRadius;
        attackRange = towerData.AttackRange;

        if (attackingSystemComponent != null)
        {
            attackingSystemComponent.ConfigureFromTowerData(towerData.Damage, towerData.AttackCooldown);
        }

        if (healthManager != null)
        {
            healthManager.Configure(towerData.MaxHealth);
        }
    }

    private void Update()
    {
        HandleTargeting();
        HandleAttack();
    }

    // =========================
    // TARGETING
    // =========================

    void HandleTargeting()
    {
        // If no target → find one
        if (currentTarget == null)
        {
            currentTarget = FindNearestTarget();
            return;
        }

        float dist = Vector3.Distance(transform.position, currentTarget.position);

        // If target out of detection → reset
        if (dist > detectionRadius)
        {
            currentTarget = null;
        }
    }

    // =========================
    // ATTACK
    // =========================

    void HandleAttack()
    {
        if (currentTarget == null)
            return;
        if (!currentTarget.gameObject.activeInHierarchy)
        {
            currentTarget = null;
            return;
        }

        if (!canAttack)
            return;
        if (attackSystem == null)
            return;

        float dist = Vector3.Distance(transform.position, currentTarget.position);

        if (dist > attackRange)
            return;

        if (!attackSystem.CanAttack())
            return;

        IDamageable dmg = currentTarget.GetComponent<IDamageable>();

        if (dmg != null)
        {
            attackSystem.Attack(dmg);
        }
    }

    public void SetCanAttack(bool value)
    {
        canAttack = value;
        if (!canAttack)
        {
            currentTarget = null;
        }
    }

    // =========================
    // FIND TARGET
    // =========================

    Transform FindNearestTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, targetLayer);

        float closest = Mathf.Infinity;
        Transform nearest = null;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Tower"))
                continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);

            if (dist < closest)
            {
                closest = dist;
                nearest = hit.transform;
            }
        }

        return nearest;
    }
}