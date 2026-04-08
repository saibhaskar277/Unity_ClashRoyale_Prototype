using UnityEngine;

[CreateAssetMenu(menuName = "Game/Tower Data")]
public class TowerData : ScriptableObject
{
    public float MaxHealth = 1000f;
    public float Damage = 50f;
    public float AttackRange = 8f;
    public float AttackCooldown = 1f;
    public float DetectionRadius = 10f;

    public GameObject ProjectilePrefab; // optional later
}