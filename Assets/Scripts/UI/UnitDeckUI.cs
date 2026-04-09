using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

public class UnitDeckUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] UnitSpawnSystem spawnSystem;
    [SerializeField] UnitConfigDatabase unitDatabase;

    [Header("Card Container")]
    [SerializeField] Transform cardContainer;
    [SerializeField] UnitCardView cardPrefab;
    [SerializeField] int deckSize = 4;

    [Header("Elixir UI")]
    [SerializeField] TextMeshProUGUI elixirText;
    [SerializeField] Slider elixirSlider;

    readonly List<UnitCardView> cards = new List<UnitCardView>();
    readonly List<UnitData> allUnits = new List<UnitData>();
    readonly List<UnitData> visibleUnits = new List<UnitData>();
    readonly Queue<UnitData> drawQueue = new Queue<UnitData>();
    float lastElixirValue;
    bool wasPlacing;
    UnitData lastSelectedUnit;

    void Start()
    {
        BuildCards();
        if (spawnSystem != null)
        {
            spawnSystem.OnUnitSpawned += HandleUnitSpawned;
        }
        lastElixirValue = GetCurrentElixirFromSystem();
        wasPlacing = GetIsPlacingFromSystem();
        lastSelectedUnit = GetSelectedUnitFromSystem();
        RefreshElixir(lastElixirValue, GetMaxElixirFromSystem());
        RefreshCards();
    }

    void OnDestroy()
    {
        if (spawnSystem != null)
        {
            spawnSystem.OnUnitSpawned -= HandleUnitSpawned;
        }
    }

    void Update()
    {
        if (spawnSystem == null)
            return;

        float currentElixir = GetCurrentElixirFromSystem();
        float maxElixir = GetMaxElixirFromSystem();
        bool isPlacing = GetIsPlacingFromSystem();
        UnitData selectedUnit = GetSelectedUnitFromSystem();

        if (!Mathf.Approximately(currentElixir, lastElixirValue))
        {
            RefreshElixir(currentElixir, maxElixir);
        }

        if (selectedUnit != lastSelectedUnit || isPlacing != wasPlacing)
        {
            RefreshCards();
        }

        lastElixirValue = currentElixir;
        wasPlacing = isPlacing;
        lastSelectedUnit = selectedUnit;
    }

    void BuildCards()
    {
        if (spawnSystem == null || unitDatabase == null || cardContainer == null || cardPrefab == null)
        {
            Debug.LogWarning("UnitDeckUI is missing references.", this);
            return;
        }

        cards.Clear();
        allUnits.Clear();
        visibleUnits.Clear();
        drawQueue.Clear();

        IReadOnlyList<UnitData> units = GetUnitsFromDatabase();
        if (units == null || units.Count == 0)
        {
            Debug.LogWarning("UnitConfigDatabase has no units.", this);
            return;
        }

        foreach (UnitData unit in units)
        {
            if (unit == null || GetUnitPrefab(unit) == null)
                continue;

            if (allUnits.Exists(x => x.UnitId == unit.UnitId))
                continue;

            allUnits.Add(unit);
        }

        ShuffleUnits(allUnits);

        int targetVisibleCount = Mathf.Min(Mathf.Max(1, deckSize), allUnits.Count);
        for (int i = 0; i < targetVisibleCount; i++)
        {
            visibleUnits.Add(allUnits[i]);
            UnitCardView card = Instantiate(cardPrefab, cardContainer);
            card.Initialize(visibleUnits[i], spawnSystem);
            cards.Add(card);
        }

        for (int i = targetVisibleCount; i < allUnits.Count; i++)
        {
            drawQueue.Enqueue(allUnits[i]);
        }
    }

    void ShuffleUnits(List<UnitData> units)
    {
        for (int i = units.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            UnitData temp = units[i];
            units[i] = units[randomIndex];
            units[randomIndex] = temp;
        }
    }

    void RefreshElixir(float current, float max)
    {
        if (elixirText != null)
            elixirText.text = $"Elixir: {Mathf.FloorToInt(current)}/{Mathf.FloorToInt(max)}";

        if (elixirSlider != null)
            elixirSlider.value = max > 0f ? current / max : 0f;

        RefreshCards();
    }

    void CycleDeckAfterSpawn(UnitData spawnedUnit)
    {
        if (spawnedUnit == null)
            return;
        if (drawQueue.Count == 0)
            return;

        int visibleIndex = visibleUnits.FindIndex(x => x != null && x.UnitId == spawnedUnit.UnitId);
        if (visibleIndex < 0)
            return;

        UnitData replacement = drawQueue.Dequeue();
        if (replacement == null)
            return;

        drawQueue.Enqueue(spawnedUnit);
        visibleUnits[visibleIndex] = replacement;
        cards[visibleIndex].Initialize(replacement, spawnSystem);
        RefreshCards();
    }

    void RefreshCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] != null)
                cards[i].RefreshState();
        }
    }

    IReadOnlyList<UnitData> GetUnitsFromDatabase()
    {
        if (unitDatabase == null)
            return null;

        return unitDatabase.GetAll();
    }

    GameObject GetUnitPrefab(UnitData data)
    {
        return data.UnitPrefab;
    }

    float GetCurrentElixirFromSystem()
    {
        return spawnSystem.CurrentElixir;
    }

    float GetMaxElixirFromSystem()
    {
        return spawnSystem.MaxElixir;
    }

    bool GetIsPlacingFromSystem()
    {
        return spawnSystem.IsPlacing;
    }

    UnitData GetSelectedUnitFromSystem()
    {
        return spawnSystem.SelectedUnitData;
    }

    void HandleUnitSpawned(UnitData spawnedUnit)
    {
        CycleDeckAfterSpawn(spawnedUnit);
    }
}
