using System;
using UnityEngine;

public class PooledProjectile : MonoBehaviour
{
    IDamageable target;
    float damage;
    float speed;
    float lifeTime;
    float age;
    Action<PooledProjectile> returnToPool;
    bool isActiveProjectile;

    public void Launch(IDamageable newTarget, float newDamage, float newSpeed, float newLifeTime, Action<PooledProjectile> onReturn)
    {
        target = newTarget;
        damage = newDamage;
        speed = newSpeed;
        lifeTime = Mathf.Max(0.1f, newLifeTime);
        age = 0f;
        returnToPool = onReturn;
        isActiveProjectile = true;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!isActiveProjectile)
            return;

        age += Time.deltaTime;
        if (age >= lifeTime)
        {
            ReturnToPool();
            return;
        }

        if (target == null || target.Target == null || !target.Target.gameObject.activeInHierarchy)
        {
            ReturnToPool();
            return;
        }

        Vector3 toTarget = target.Target.position - transform.position;
        float step = speed * Time.deltaTime;

        if (toTarget.sqrMagnitude <= step * step)
        {
            target.TakeDamage(damage);
            ReturnToPool();
            return;
        }

        transform.position += toTarget.normalized * step;
    }

    void ReturnToPool()
    {
        isActiveProjectile = false;
        returnToPool?.Invoke(this);
    }
}
