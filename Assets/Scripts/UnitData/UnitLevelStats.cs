using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Clash Royale/Unit Level Stats")]
public class UnitLevelStats : ScriptableObject
{
    [System.Serializable]
    public class LevelStat
    {
        public int Level;
        public float Health;
        public float Damage;
    }

    [SerializeField] private UnitId unitId;
    [SerializeField] private List<LevelStat> levels = new();

    public float GetHealth(int level)
    {
        LevelStat stat = levels.Find(x => x.Level == level);
        return stat != null ? stat.Health : 0f;
    }

    public float GetDamage(int level)
    {
        LevelStat stat = levels.Find(x => x.Level == level);
        return stat != null ? stat.Damage : 0f;
    }

    public UnitId UnitId => unitId;
}