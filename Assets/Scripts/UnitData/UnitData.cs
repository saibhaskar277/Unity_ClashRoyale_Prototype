using UnityEngine;

[CreateAssetMenu(menuName = "Game/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("Info")]
    public UnitId UnitId;
    public TargetAttackType AttackType;
    public UnitCategory UnitType;

    [Header("Card UI")]
    public Sprite CardSprite;
    [Min(0)] public int ElixirCost = 3;

    [Header("Spawn")]
    public GameObject UnitPrefab;
    public GameObject PreviewPrefab;

    [Header("Stats")]
    public float MaxHealth = 100f;
    public float Damage = 10f;
    public float AttackRange = 2f;
    public float AttackCooldown = 1f;
    public float MoveSpeed = 3.5f;

    [Header("Detection")]
    public float DetectionRadius = 8f;
}


