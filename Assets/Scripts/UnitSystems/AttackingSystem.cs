using UnityEngine;
using System.Collections.Generic;

public class AttackingSystem : MonoBehaviour, IAttack
{
    bool _isAttacking = false;

    public float damage = 10f;
    float attackCoolDown = 0.5f;

    Timer attackTimer;
    UnitId currentUnitId = UnitId.None;

    [Header("Projectile")]
    [SerializeField] UnitProjectileMapper projectileMapper;
    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] float projectileSpeed = 12f;
    [SerializeField] float projectileLifeTime = 4f;

    readonly Queue<PooledProjectile> projectilePool = new Queue<PooledProjectile>();
    GameObject projectilePrefab;
    bool useProjectileAttack;

    public TargetAttackType AttackType => attackType;
    [SerializeField] TargetAttackType attackType;

    private void Awake()
    {
        attackTimer = new Timer(attackCoolDown);
        if (projectileSpawnPoint == null)
        {
            projectileSpawnPoint = transform;
        }
    }

    private void Update()
    {
        if (_isAttacking)
        {
            attackTimer.Tick();

            if (attackTimer.IsFinished)
            {
                _isAttacking = false;
                attackTimer.Reset();
            }
        }
    }

    public void Configure(UnitData data)
    {
        damage = data.Damage;
        attackCoolDown = data.AttackCooldown;
        currentUnitId = data.UnitId;
        ResolveProjectileConfig();

        attackTimer = new Timer(attackCoolDown);
    }

    public void ConfigureFromTowerData(float newDamage, float newAttackCooldown)
    {
        damage = newDamage;
        attackCoolDown = newAttackCooldown;
        currentUnitId = UnitId.None;
        projectilePrefab = null;
        useProjectileAttack = false;
        attackTimer = new Timer(attackCoolDown);
    }

    public void Attack(IDamageable target)
    {
        if (!CanAttack())
            return;

        _isAttacking = true;
        if (useProjectileAttack && projectilePrefab != null)
        {
            FireProjectile(target);
            return;
        }

        target.TakeDamage(damage);
    }

    public bool CanAttack()
    {
        return !_isAttacking;
    }

    void ResolveProjectileConfig()
    {
        projectilePrefab = projectileMapper != null ? projectileMapper.GetProjectilePrefab(currentUnitId) : null;
        useProjectileAttack = projectilePrefab != null;
    }

    void FireProjectile(IDamageable target)
    {
        if (target == null || target.Target == null)
            return;

        PooledProjectile projectile = GetProjectileFromPool();
        projectile.transform.position = projectileSpawnPoint != null ? projectileSpawnPoint.position : transform.position;
        projectile.Launch(target, damage, projectileSpeed, projectileLifeTime, ReturnProjectileToPool);
    }

    PooledProjectile GetProjectileFromPool()
    {
        while (projectilePool.Count > 0)
        {
            PooledProjectile pooled = projectilePool.Dequeue();
            if (pooled != null)
                return pooled;
        }

        GameObject instance = Instantiate(projectilePrefab);
        PooledProjectile projectile = instance.GetComponent<PooledProjectile>();
        if (projectile == null)
        {
            projectile = instance.AddComponent<PooledProjectile>();
        }

        return projectile;
    }

    void ReturnProjectileToPool(PooledProjectile projectile)
    {
        if (projectile == null)
            return;

        projectile.gameObject.SetActive(false);
        projectilePool.Enqueue(projectile);
    }
}