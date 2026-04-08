using UnityEngine;

public class UnitPoolMember : MonoBehaviour
{
    [SerializeField] UnitId unitId = UnitId.None;

    public UnitId UnitId => unitId;
    public Vector3 LastSpawnPosition { get; private set; }

    public void SetUnitId(UnitId id)
    {
        unitId = id;
    }

    public void MarkSpawnPosition(Vector3 position)
    {
        LastSpawnPosition = position;
    }
}
