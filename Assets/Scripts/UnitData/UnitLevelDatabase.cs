using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Unit Level Config Database")]
public class UnitLevelDatabase : ScriptableObject
{

    [SerializeField] private List<UnitLevelStats> units;

    private Dictionary<UnitId, UnitLevelStats> lookup;

    public void Initialize()
    {
        lookup = new Dictionary<UnitId, UnitLevelStats>();

        foreach (var unit in units)
        {
            if (!lookup.ContainsKey(unit.UnitId))
            {
                lookup.Add(unit.UnitId, unit);
            }
        }
    }

    public UnitLevelStats Get(UnitId id)
    {
        if (lookup == null)
            Initialize();

        if (lookup.TryGetValue(id, out var data))
            return data;

        Debug.LogError($"UnitData not found for {id}");
        return null;
    }






}



