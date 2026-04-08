using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using TMPro;
public class UnitCardView : MonoBehaviour
{
    [SerializeField] Image unitIcon;
    [SerializeField] TextMeshProUGUI unitNameText;
    [SerializeField] TextMeshProUGUI elixirText;
    [SerializeField] Button selectButton;
    [SerializeField] Image selectionHighlight;

    UnitData boundData;
    UnitSpawnSystem spawnSystem;

    public void Bind(UnitData data, UnitSpawnSystem system)
    {
        boundData = data;
        spawnSystem = system;

        if (unitIcon != null)
            unitIcon.sprite = boundData != null ? GetCardSprite(boundData) : null;

        if (unitNameText != null)
            unitNameText.text = boundData != null ? boundData.UnitId.ToString() : "None";

        if (elixirText != null)
            elixirText.text = boundData != null ? GetElixirCost(boundData).ToString() : "-";

        if (selectButton != null)
        {
            selectButton.onClick.RemoveListener(OnSelectClicked);
            selectButton.onClick.AddListener(OnSelectClicked);
        }

        RefreshState();
    }

    public void RefreshState()
    {
        bool canSpawn = spawnSystem != null && boundData != null && CanSpawnFromSystem();
        bool isSelected = spawnSystem != null && IsSelectedFromSystem();

        if (selectButton != null)
            selectButton.interactable = canSpawn;

        if (selectionHighlight != null)
            selectionHighlight.enabled = isSelected;
    }

    void OnSelectClicked()
    {
        if (spawnSystem == null || boundData == null)
            return;

        StartPlacementFromSystem();
    }

    Sprite GetCardSprite(UnitData data)
    {
        FieldInfo field = data.GetType().GetField("CardSprite");
        return field != null ? field.GetValue(data) as Sprite : null;
    }

    int GetElixirCost(UnitData data)
    {
        FieldInfo field = data.GetType().GetField("ElixirCost");
        if (field == null)
            return 0;

        object value = field.GetValue(data);
        return value is int intValue ? intValue : 0;
    }

    bool CanSpawnFromSystem()
    {
        MethodInfo method = spawnSystem.GetType().GetMethod("CanSpawn");
        if (method == null)
            return true;

        object result = method.Invoke(spawnSystem, new object[] { boundData });
        return result is bool boolResult && boolResult;
    }

    bool IsSelectedFromSystem()
    {
        PropertyInfo selectedProp = spawnSystem.GetType().GetProperty("SelectedUnitData");
        PropertyInfo placingProp = spawnSystem.GetType().GetProperty("IsPlacing");

        object selected = selectedProp?.GetValue(spawnSystem);
        object isPlacing = placingProp?.GetValue(spawnSystem);

        bool placing = isPlacing is bool boolValue && boolValue;
        return placing && selected == (object)boundData;
    }

    void StartPlacementFromSystem()
    {
        MethodInfo method = spawnSystem.GetType().GetMethod("StartPlacement");
        if (method == null)
            return;

        method.Invoke(spawnSystem, new object[] { boundData });
    }
}
