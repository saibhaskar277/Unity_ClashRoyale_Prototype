using UnityEngine;
using UnityEngine.AI;

public class UnitConfigMapper : MonoBehaviour
{
    [SerializeField] UnitId unitId;
    [SerializeField] UnitConfigDatabase database;

    UnitData data;

    private void Awake()
    {
        if (database == null)
        {
            Debug.LogError($"Missing {nameof(UnitConfigDatabase)} on {name}", this);
            return;
        }

        data = database.Get(unitId);
        if (data == null)
        {
            Debug.LogError($"No {nameof(UnitData)} found for {unitId} on {name}", this);
            return;
        }

        ApplyConfig();
    }

    void ApplyConfig()
    {
        // Movement
        var agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = data.MoveSpeed;
        }

        // State Controller
        var state = GetComponent<UnitStateController>();
        if (state != null)
        {
            state.AttackRange = data.AttackRange;
            state.DetectionRadius = data.DetectionRadius;
            state.TargetAttackType = data.AttackType;
        }

        // Attack System
        var attack = GetComponent<AttackingSystem>();
        if (attack != null)
        {
            attack.Configure(data);
        }

        // Health
        var health = GetComponent<HealthManager>();
        if (health != null)
        {
            health.Configure(data);
        }
    }
}