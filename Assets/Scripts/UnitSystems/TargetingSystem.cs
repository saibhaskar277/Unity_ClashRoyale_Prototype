using UnityEngine;

public class TargetingSystem : MonoBehaviour, ITargetingStrategy
{
    public IDamageable GetTarget(Transform self, float radius, LayerMask layer, TargetAttackType canAttack)
    {
        Collider[] hits = Physics.OverlapSphere(self.position, radius, layer);

        float closest = Mathf.Infinity;
        IDamageable nearest = null;

        foreach (var hit in hits)
        {
            if(hit.gameObject.CompareTag("Tower"))
                continue;

            var targetHealth = hit.GetComponent<IDamageable>();

            if (targetHealth != null)
            {
                var targetCategory = targetHealth.Category;

                if (!CanHit(canAttack, targetCategory))
                    continue;

                float dist = Vector3.Distance(self.position, hit.transform.position);

                if (dist < closest)
                {
                    closest = dist;
                    nearest = targetHealth;
                }
            }
        }

        return nearest;
    }

    public IDamageable GetTower(Transform self, float radius, LayerMask layer)
    {
        Collider[] hits = Physics.OverlapSphere(self.position, radius, layer);

        float closest = Mathf.Infinity;
        IDamageable nearest = null;

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Tower"))
                continue;

            float dist = Vector3.Distance(self.position, hit.transform.position);

            if (dist < closest)
            {
                IDamageable target = hit.GetComponent<IDamageable>();

                if (target != null)
                {
                    closest = dist;
                    nearest = target;
                }
            }
        }

        return nearest;
    }


    bool CanHit(TargetAttackType canAttack, UnitCategory targetCategory)
    {
        switch (canAttack)
        {
            case TargetAttackType.Both:
                return true;

            case TargetAttackType.Ground:
                return targetCategory == UnitCategory.Grounded;

            case TargetAttackType.Air:
                return targetCategory == UnitCategory.Air;
        }

        return false;
    }
}