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
    float lastElixirValue;
    bool wasPlacing;
    UnitData lastSelectedUnit;

    void Start()
    {
        BuildCards();
        lastElixirValue = GetCurrentElixirFromSystem();
        wasPlacing = GetIsPlacingFromSystem();
        lastSelectedUnit = GetSelectedUnitFromSystem();
        RefreshElixir(lastElixirValue, GetMaxElixirFromSystem());
        RefreshCards();
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

        bool endedPlacement = wasPlacing && !isPlacing;
        bool elixirSpent = currentElixir < lastElixirValue;
        if (endedPlacement && elixirSpent && lastSelectedUnit != null)
        {
            CycleDeckAfterSpawn(lastSelectedUnit);
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
            card.Bind(visibleUnits[i], spawnSystem);
            cards.Add(card);
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
        if (allUnits.Count <= visibleUnits.Count)
            return;

        int visibleIndex = visibleUnits.FindIndex(x => x != null && x.UnitId == spawnedUnit.UnitId);
        if (visibleIndex < 0)
            return;

        int allIndex = allUnits.FindIndex(x => x != null && x.UnitId == spawnedUnit.UnitId);
        if (allIndex < 0)
            return;

        UnitData moved = allUnits[allIndex];
        allUnits.RemoveAt(allIndex);
        allUnits.Add(moved);

        UnitData replacement = null;
        for (int i = visibleUnits.Count; i < allUnits.Count; i++)
        {
            UnitData candidate = allUnits[i];
            if (!visibleUnits.Exists(x => x != null && candidate != null && x.UnitId == candidate.UnitId))
            {
                replacement = candidate;
                break;
            }
        }

        if (replacement == null)
            return;

        visibleUnits[visibleIndex] = replacement;
        cards[visibleIndex].Bind(replacement, spawnSystem);
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

    float GetCurrentElixirFromSystem()
    {
        return GetFloatProperty(spawnSystem, "CurrentElixir", 0f);
    }

    float GetMaxElixirFromSystem()
    {
        return GetFloatProperty(spawnSystem, "MaxElixir", 10f);
    }

    bool GetIsPlacingFromSystem()
    {
        PropertyInfo prop = spawnSystem != null ? spawnSystem.GetType().GetProperty("IsPlacing") : null;
        object value = prop?.GetValue(spawnSystem);
        return value is bool flag && flag;
    }

    UnitData GetSelectedUnitFromSystem()
    {
        PropertyInfo prop = spawnSystem != null ? spawnSystem.GetType().GetProperty("SelectedUnitData") : null;
        object value = prop?.GetValue(spawnSystem);
        return value as UnitData;
    }

    float GetFloatProperty(object target, string propertyName, float fallback)
    {
        if (target == null)
            return fallback;

        PropertyInfo prop = target.GetType().GetProperty(propertyName);
        if (prop == null)
            return fallback;

        object value = prop.GetValue(target);
        if (value is float floatValue)
            return floatValue;

        return fallback;
    }
}
