 using System.Collections;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.MessageBox;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedTextTMP : MonoBehaviour
{
    [SerializeField] private LocalizationKey key;

    private TextMeshProUGUI textComponent;

    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        StartCoroutine(WaitForManager());
    }

    IEnumerator WaitForManager()
    {
        yield return new WaitUntil(()=> LocalizationManager.Instance != null);

        LocalizationManager.Instance.OnLanguageChanged += UpdateText;

        if (LocalizationManager.Instance.IsInitialized)
        {
            UpdateText();
        }

    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
    }

    private void UpdateText()
    {
        var manager = LocalizationManager.Instance;

        if (manager == null || !manager.IsInitialized) return;

        textComponent.text = manager.GetText(key);

        textComponent.font = manager.CurrentFontStyle;
    }

}