using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/UnitProjectileMapper")]
public class UnitProjectileMapper : ScriptableObject
{
    public List<UnitProjectile> projectileDatas;

    public GameObject GetProjectilePrefab(UnitId type)
    {
        if (projectileDatas == null)
            return null;

        UnitProjectile data = projectileDatas.Find(x => x != null && x.unitID == type);
        return data != null ? data.projectilePrefab : null;
    }
}

[System.Serializable]
public class UnitProjectile
{
    public UnitId unitID;
    public GameObject projectilePrefab;
}
