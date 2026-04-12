
using UnityEngine;

public interface IUnit
{
    UnitTeam Type { get; }
}



public interface IDamageable
{
    public UnitCategory Category { get; }
    public Transform Target { get; }
    public void TakeDamage(float damage);
}


public interface ITargetingStrategy
{
    IDamageable GetTarget(Transform self, float radius, LayerMask layer, TargetAttackType canAttack);


    IDamageable GetTower(Transform self, float radius, LayerMask layer);
}


public interface IAttack
{
    bool CanAttack();
    void Attack(IDamageable target);
    TargetAttackType AttackType { get; }


}

public interface ICardAbility
{
    bool CanUse();
    void Use(UnitStateController owner, IDamageable target);
}

