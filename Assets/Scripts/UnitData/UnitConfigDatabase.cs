using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Unit Config Database")]
public class UnitConfigDatabase : ScriptableObject
{
    [SerializeField] private List<UnitData> units;

    private Dictionary<UnitId, UnitData> lookup;

    public void Initialize()
    {
        lookup = new Dictionary<UnitId, UnitData>();

        foreach (var unit in units)
        {
            if (!lookup.ContainsKey(unit.UnitId))
            {
                lookup.Add(unit.UnitId, unit);
            }
        }
    }

    public UnitData Get(UnitId id)
    {
        if (lookup == null)
            Initialize();

        if (lookup.TryGetValue(id, out var data))
            return data;

        Debug.LogError($"UnitData not found for {id}");
        return null;
    }

    public IReadOnlyList<UnitData> GetAll()
    {
        return units;
    }
}