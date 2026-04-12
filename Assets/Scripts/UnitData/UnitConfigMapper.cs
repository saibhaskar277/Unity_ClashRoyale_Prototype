using UnityEngine;
using UnityEngine.AI;

public class UnitConfigMapper : MonoBehaviour
{
    [SerializeField] UnitConfigDatabase database;
    [SerializeField] UnitLevelDatabase levelDatabase;

    UnitData data;

    UnitId id;

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
        var unitLevel = levelDatabase.Get(id);
        if (unitLevel != null)
        {
            if (attack != null)
            {
                attack.Configure(unitLevel.GetDamage(data.currentLevel), data.AttackCooldown);
            }

            // Health
            var health = GetComponent<HealthManager>();
            if (health != null)
            {
                health.Configure(unitLevel.GetHealth(data.currentLevel));
            }
        }
    }
}