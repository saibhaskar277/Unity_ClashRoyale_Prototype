using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Clash Royale/Player Card Levels")]
public class PlayerCardLevelDatabase : ScriptableObject
{
    [System.Serializable]
    public class CardLevelEntry
    {
        public UnitId UnitId;
        public int Level = 1;
    }

    [SerializeField] private List<CardLevelEntry> cardLevels = new();

    public int GetLevel(UnitId unitId)
    {
        CardLevelEntry entry = cardLevels.Find(x => x.UnitId == unitId);
        return entry != null ? entry.Level : 1;
    }
}