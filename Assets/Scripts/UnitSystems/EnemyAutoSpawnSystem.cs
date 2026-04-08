using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAutoSpawnSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] UnitConfigDatabase unitDatabase;

    [Header("Deck")]
    [SerializeField] int deckSize = 4;

    [Header("Enemy Spawn Area")]
    [SerializeField] LayerMask enemySpawnLayer;
    [SerializeField] float maxSampleDistance = 0.5f;
    [SerializeField] int spawnPointSearchAttempts = 20;

    [Header("Spawn Timing")]
    [SerializeField] float minSpawnInterval = 1.5f;
    [SerializeField] float maxSpawnInterval = 3.5f;

    [Header("Elixir")]
    [SerializeField] float maxElixir = 10f;
    [SerializeField] float startElixir = 5f;
    [SerializeField] float elixirRegenPerSecond = 1f;

    readonly List<UnitData> allUnits = new List<UnitData>();
    readonly List<UnitData> activeDeck = new List<UnitData>();
    readonly List<Collider> spawnAreaColliders = new List<Collider>();

    float currentElixir;
    float spawnTimer;
    float nextSpawnDelay;

    public float CurrentElixir => currentElixir;
    public float MaxElixir => maxElixir;

    void Start()
    {
        BuildDeck();
        CacheSpawnAreaColliders();

        currentElixir = Mathf.Clamp(startElixir, 0f, maxElixir);
        ResetSpawnTimer();
    }

    void Update()
    {
        RegenerateElixir();

        if (activeDeck.Count == 0 || spawnAreaColliders.Count == 0)
            return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer < nextSpawnDelay)
            return;

        TryAutoSpawn();
        ResetSpawnTimer();
    }

    void BuildDeck()
    {
        allUnits.Clear();
        activeDeck.Clear();

        if (unitDatabase == null)
            return;

        IReadOnlyList<UnitData> units = GetUnitsFromDatabase();
        if (units == null)
            return;

        for (int i = 0; i < units.Count; i++)
        {
            UnitData unit = units[i];
            if (unit == null || GetUnitPrefab(unit) == null)
                continue;

            if (allUnits.Exists(x => x.UnitId == unit.UnitId))
                continue;

            allUnits.Add(unit);
        }

        Shuffle(allUnits);

        int activeCount = Mathf.Min(Mathf.Max(1, deckSize), allUnits.Count);
        for (int i = 0; i < activeCount; i++)
        {
            activeDeck.Add(allUnits[i]);
        }
    }

    void CacheSpawnAreaColliders()
    {
        spawnAreaColliders.Clear();
        Collider[] sceneColliders = FindObjectsByType<Collider>(FindObjectsSortMode.None);

        for (int i = 0; i < sceneColliders.Length; i++)
        {
            Collider col = sceneColliders[i];
            if (col == null || !col.enabled)
                continue;

            int layerBit = 1 << col.gameObject.layer;
            if ((enemySpawnLayer.value & layerBit) == 0)
                continue;

            spawnAreaColliders.Add(col);
        }
    }

    void TryAutoSpawn()
    {
        List<int> spawnableIndices = new List<int>();
        for (int i = 0; i < activeDeck.Count; i++)
        {
            UnitData data = activeDeck[i];
            if (data != null && GetUnitPrefab(data) != null && currentElixir >= GetElixirCost(data))
            {
                spawnableIndices.Add(i);
            }
        }

        if (spawnableIndices.Count == 0)
            return;

        int selectedDeckIndex = spawnableIndices[Random.Range(0, spawnableIndices.Count)];
        UnitData selectedData = activeDeck[selectedDeckIndex];

        if (!TryGetRandomSpawnPosition(out Vector3 spawnPosition))
            return;

        UnitPoolManager.Instance.Spawn(selectedData, spawnPosition, Quaternion.identity, UnitTeam.EnemyTeam);

        currentElixir = Mathf.Max(0f, currentElixir - GetElixirCost(selectedData));
        CycleDeckAfterSpawn(selectedData, selectedDeckIndex);
    }

    bool TryGetRandomSpawnPosition(out Vector3 spawnPosition)
    {
        spawnPosition = Vector3.zero;
        if (spawnAreaColliders.Count == 0)
            return false;

        for (int attempt = 0; attempt < spawnPointSearchAttempts; attempt++)
        {
            Collider area = spawnAreaColliders[Random.Range(0, spawnAreaColliders.Count)];
            Bounds b = area.bounds;
            Vector3 sample = new Vector3(
                Random.Range(b.min.x, b.max.x),
                b.max.y + 30f,
                Random.Range(b.min.z, b.max.z)
            );

            if (!Physics.Raycast(sample, Vector3.down, out RaycastHit hit, 100f, enemySpawnLayer))
                continue;

            Vector3 point = hit.point;
            if (NavMesh.SamplePosition(point, out NavMeshHit navHit, maxSampleDistance, NavMesh.AllAreas))
            {
                if (Vector3.Distance(point, navHit.position) <= maxSampleDistance)
                {
                    spawnPosition = navHit.position;
                    return true;
                }
            }
        }

        return false;
    }

    void CycleDeckAfterSpawn(UnitData spawnedData, int deckIndex)
    {
        if (allUnits.Count <= activeDeck.Count)
            return;

        int allIndex = allUnits.FindIndex(x => x != null && x.UnitId == spawnedData.UnitId);
        if (allIndex < 0)
            return;

        UnitData moved = allUnits[allIndex];
        allUnits.RemoveAt(allIndex);
        allUnits.Add(moved);

        UnitData replacement = null;
        for (int i = activeDeck.Count; i < allUnits.Count; i++)
        {
            UnitData candidate = allUnits[i];
            if (!activeDeck.Exists(x => x != null && candidate != null && x.UnitId == candidate.UnitId))
            {
                replacement = candidate;
                break;
            }
        }

        if (replacement != null && deckIndex >= 0 && deckIndex < activeDeck.Count)
        {
            activeDeck[deckIndex] = replacement;
        }
    }

    void RegenerateElixir()
    {
        if (currentElixir >= maxElixir)
            return;

        currentElixir = Mathf.Min(maxElixir, currentElixir + (elixirRegenPerSecond * Time.deltaTime));
    }

    void ResetSpawnTimer()
    {
        spawnTimer = 0f;
        nextSpawnDelay = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    void Shuffle(List<UnitData> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            UnitData temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    IReadOnlyList<UnitData> GetUnitsFromDatabase()
    {
        if (unitDatabase == null)
            return null;

        MethodInfo getAllMethod = unitDatabase.GetType().GetMethod("GetAll");
        if (getAllMethod != null)
        {
            object result = getAllMethod.Invoke(unitDatabase, null);
            if (result is IReadOnlyList<UnitData> readOnlyList)
                return readOnlyList;
            if (result is List<UnitData> list)
                return list;
        }

        FieldInfo unitsField = unitDatabase.GetType().GetField("units", BindingFlags.NonPublic | BindingFlags.Instance);
        if (unitsField != null && unitsField.GetValue(unitDatabase) is List<UnitData> units)
            return units;

        return null;
    }

    GameObject GetUnitPrefab(UnitData data)
    {
        FieldInfo prefabField = data.GetType().GetField("UnitPrefab");
        return prefabField != null ? prefabField.GetValue(data) as GameObject : null;
    }

    int GetElixirCost(UnitData data)
    {
        FieldInfo elixirField = data.GetType().GetField("ElixirCost");
        if (elixirField == null)
            return 0;

        object value = elixirField.GetValue(data);
        return value is int intValue ? intValue : 0;
    }
}
