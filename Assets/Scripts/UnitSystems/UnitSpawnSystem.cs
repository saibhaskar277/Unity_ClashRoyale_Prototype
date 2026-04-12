using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEngine.EventSystems;

public class UnitSpawnSystem : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float maxSampleDistance = 0.1f;
    [SerializeField] UnitTeam spawnTeam = UnitTeam.PlayerTeam;

    [Header("Elixir")]
    [SerializeField] float maxElixir = 10f;
    [SerializeField] float startElixir = 5f;
    [SerializeField] float elixirRegenPerSecond = 1f;

    UnitData selectedUnitData;
    GameObject previewInstance;
    bool isPlacing;
    float currentElixir;
    float elixirRegenMultiplier = 1f;

    [SerializeField] private PlayerCardLevelDatabase playerLevels;

    public float CurrentElixir => currentElixir;
    public float MaxElixir => maxElixir;
    public UnitData SelectedUnitData => selectedUnitData;
    public bool IsPlacing => isPlacing;
    public float CurrentElixirRegenPerSecond => elixirRegenPerSecond * elixirRegenMultiplier;

    public event Action<float, float> OnElixirChanged;
    public event Action<UnitData> OnSelectedUnitChanged;
    public event Action<UnitData> OnUnitSpawned;

    void Start()
    {
        currentElixir = Mathf.Clamp(startElixir, 0f, maxElixir);
        RaiseElixirChanged();
    }

    void Update()
    {
        RegenerateElixir();

        if (!isPlacing)
            return;

        UpdatePreview();

        if (Input.GetMouseButtonDown(0))
        {
            TrySpawn();
        }

        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    public bool CanSpawn(UnitData data)
    {
        if (data == null || data.UnitPrefab == null)
            return false;

        return currentElixir >= data.ElixirCost;
    }

    public void StartPlacement(UnitData data)
    {
        if (data == null)
            return;
        if (!CanSpawn(data))
            return;

        selectedUnitData = data;
        isPlacing = true;

        if (previewInstance != null)
            Destroy(previewInstance);

        if (selectedUnitData.PreviewPrefab != null)
        {
            previewInstance = Instantiate(selectedUnitData.PreviewPrefab);
        }

        OnSelectedUnitChanged?.Invoke(selectedUnitData);
    }

    public void CancelPlacement()
    {
        isPlacing = false;
        selectedUnitData = null;

        if (previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null;
        }

        OnSelectedUnitChanged?.Invoke(null);
    }

    void RegenerateElixir()
    {
        if (currentElixir >= maxElixir)
            return;

        currentElixir = Mathf.Min(maxElixir, currentElixir + (CurrentElixirRegenPerSecond * Time.deltaTime));
        RaiseElixirChanged();
    }

    public void SetElixirRegenMultiplier(float multiplier)
    {
        elixirRegenMultiplier = Mathf.Max(0f, multiplier);
    }

    void UpdatePreview()
    {
        if (selectedUnitData == null || Camera.main == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
            return;

        Vector3 point = hit.point;
        bool requiresNavMesh = RequiresNavMeshPlacement(selectedUnitData);
        Vector3 navPosition = point;
        bool isValid = !requiresNavMesh || TryGetNavMeshPosition(point, out navPosition);
        if (!requiresNavMesh)
        {
            navPosition = point;
        }

        if (previewInstance != null)
        {
            previewInstance.transform.position = isValid ? navPosition : point;
            SetPreviewColor(isValid);
        }
    }

    void TrySpawn()
    {
        if (selectedUnitData == null || selectedUnitData.UnitPrefab == null || Camera.main == null)
            return;
        if (!CanSpawn(selectedUnitData))
            return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
            return;

        Vector3 spawnPosition = hit.point;
        if (RequiresNavMeshPlacement(selectedUnitData))
        {
            if (!TryGetNavMeshPosition(hit.point, out spawnPosition))
                return;
        }

        int level = playerLevels.GetLevel(selectedUnitData.UnitId);

        UnitSpawnData spawnData = new UnitSpawnData
        {
            currentUnitType = spawnTeam,
            unitData = selectedUnitData,
            level = level
        };

        UnitPoolManager.Instance.Spawn(selectedUnitData, spawnPosition, Quaternion.identity, spawnData.currentUnitType);
           


        SpendElixir(selectedUnitData.ElixirCost);
        OnUnitSpawned?.Invoke(selectedUnitData);
        EventManager.RaiseEvent(new UnitSpawnedEvent
        {
            UnitData = selectedUnitData,
            Team = spawnTeam
        });
        CancelPlacement();
    }

    bool TryGetNavMeshPosition(Vector3 point, out Vector3 navPosition)
    {
        navPosition = point;
        if (NavMesh.SamplePosition(point, out NavMeshHit navHit, maxSampleDistance, NavMesh.AllAreas))
        {
            float dist = Vector3.Distance(point, navHit.position);
            if (dist <= maxSampleDistance)
            {
                navPosition = navHit.position;
                return true;
            }
        }

        return false;
    }

    bool RequiresNavMeshPlacement(UnitData data)
    {
        if (data == null)
            return true;

        return data.UnitType == UnitCategory.Grounded;
    }

    void SetPreviewColor(bool valid)
    {
        if (previewInstance == null)
            return;

        var renderer = previewInstance.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = valid ? Color.green : Color.red;
        }
    }

    void SpendElixir(float amount)
    {
        currentElixir = Mathf.Max(0f, currentElixir - amount);
        RaiseElixirChanged();
    }

    void RaiseElixirChanged()
    {
        OnElixirChanged?.Invoke(currentElixir, maxElixir);
        EventManager.RaiseEvent(new ElixirChangedEvent
        {
            CurrentElixir = currentElixir,
            MaxElixir = maxElixir,
            RegenPerSecond = CurrentElixirRegenPerSecond
        });
    }
}
