using UnityEngine;

[CreateAssetMenu(menuName = "Game/Unit Data")]
public class UnitData : ScriptableObject
{
    public UnitId UnitId;
    public GameObject UnitPrefab;
    public GameObject PreviewPrefab;
    public Sprite CardSprite;
    [Header("Static Gameplay")]
    public float MoveSpeed;
    public float AttackRange;
    public float DetectionRadius;
    public float AttackCooldown;
    public int ElixirCost;
    public UnitCategory UnitType;
    public TargetAttackType AttackType;
}


