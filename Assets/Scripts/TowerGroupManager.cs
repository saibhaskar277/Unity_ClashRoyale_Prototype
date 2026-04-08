using System.Collections.Generic;
using UnityEngine;

public class TowerGroupManager : MonoBehaviour
{
    [Header("Tower Setup")]
    [SerializeField] Tower mainTower;
    [SerializeField] List<Tower> sideTowers = new List<Tower>();
    [SerializeField] bool mainStartsDisabled = true;

    bool mainUnlocked;

    void Awake()
    {
        AutoAssignFromChildrenIfNeeded();

        if (mainTower != null && mainStartsDisabled)
        {
            mainTower.SetCanAttack(false);
        }
    }

    void Update()
    {
        if (mainUnlocked || mainTower == null)
            return;

        if (IsAnySideTowerDestroyed())
        {
            mainUnlocked = true;
            mainTower.SetCanAttack(true);
        }
    }

    bool IsAnySideTowerDestroyed()
    {
        for (int i = 0; i < sideTowers.Count; i++)
        {
            Tower sideTower = sideTowers[i];
            if (sideTower == null || !sideTower.gameObject.activeInHierarchy)
                return true;
        }

        return false;
    }

    void AutoAssignFromChildrenIfNeeded()
    {
        Tower[] childTowers = GetComponentsInChildren<Tower>(true);
        if (childTowers.Length == 0)
            return;

        if (mainTower == null)
        {
            for (int i = 0; i < childTowers.Length; i++)
            {
                if (childTowers[i] != null && !childTowers[i].CanAttack)
                {
                    mainTower = childTowers[i];
                    break;
                }
            }

            if (mainTower == null)
            {
                mainTower = childTowers[0];
            }
        }

        if (sideTowers.Count == 0)
        {
            for (int i = 0; i < childTowers.Length; i++)
            {
                Tower tower = childTowers[i];
                if (tower != null && tower != mainTower)
                {
                    sideTowers.Add(tower);
                }
            }
        }
    }
}
