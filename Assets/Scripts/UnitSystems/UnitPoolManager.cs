using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class UnitPoolManager : MonoBehaviour
{
    static UnitPoolManager instance;

    readonly Dictionary<UnitId, Queue<GameObject>> poolByUnitId = new Dictionary<UnitId, Queue<GameObject>>();

    public static UnitPoolManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("UnitPoolManager");
                instance = go.AddComponent<UnitPoolManager>();
            }

            return instance;
        }
    }

    public GameObject Spawn(UnitData data, Vector3 position, Quaternion rotation, UnitTeam team)
    {
        GameObject unitPrefab = GetUnitPrefab(data);
        if (data == null || unitPrefab == null)
            return null;

        UnitId key = data.UnitId;
        if (!poolByUnitId.TryGetValue(key, out Queue<GameObject> queue))
        {
            queue = new Queue<GameObject>();
            poolByUnitId[key] = queue;
        }

        GameObject unit = null;
        while (queue.Count > 0 && unit == null)
        {
            unit = queue.Dequeue();
        }

        if (unit == null)
        {
            unit = Instantiate(unitPrefab);
        }

        UnitPoolMember member = unit.GetComponent<UnitPoolMember>();
        if (member == null)
        {
            member = unit.AddComponent<UnitPoolMember>();
            member.GetComponent<UnitConfigMapper>().SetUnitData(data);
        }
        else
        {
            member.GetComponent<UnitConfigMapper>().ResetUnitData();
        }

        member.SetUnitId(key);
        member.MarkSpawnPosition(position);

        unit.transform.SetPositionAndRotation(position, rotation);
        unit.SetActive(true);

        UnitStateController stateController = unit.GetComponent<UnitStateController>();
        if (stateController != null)
        {
            stateController.SetUnitData(data, team);    
        }

        return unit;
    }

    public void Despawn(GameObject unit)
    {
        if (unit == null)
            return;

        UnitPoolMember member = unit.GetComponent<UnitPoolMember>();
        if (member == null)
        {
            unit.SetActive(false);
            return;
        }

        UnitId key = member.UnitId;
        if (!poolByUnitId.TryGetValue(key, out Queue<GameObject> queue))
        {
            queue = new Queue<GameObject>();
            poolByUnitId[key] = queue;
        }

        unit.SetActive(false);
        queue.Enqueue(unit);
    }

    GameObject GetUnitPrefab(UnitData data)
    {
        if (data == null)
            return null;

        FieldInfo prefabField = data.GetType().GetField("UnitPrefab");
        return prefabField != null ? prefabField.GetValue(data) as GameObject : null;
    }
}
