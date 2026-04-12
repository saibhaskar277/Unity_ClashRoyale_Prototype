using UnityEngine;
using UnityEngine.AI;

public class UnitConfigMapper : MonoBehaviour
{
    [SerializeField] UnitConfigDatabase database;

    UnitData data;

    [SerializeField] UnitId id;

    public void SetUnitData(UnitData data)
    {
        this.data = data;
        id = data.UnitId;
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