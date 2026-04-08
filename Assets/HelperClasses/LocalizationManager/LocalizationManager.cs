using System;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    public List<LocalizationDataSO> languages;


    public bool IsInitialized { get; private set; }

    public TMP_FontAsset CurrentFontStyle { get; private set; }

    public Language CurrentLanguage;

    private Dictionary<string, string> localizedText = new();

    public event Action OnLanguageChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        var currentLanguage = languages.Find(x => x.languageName == CurrentLanguage);

        LoadLanguage(currentLanguage);

    }

    public void LoadLanguage(LocalizationDataSO data)
    {
        if (data == null)
        {
            Debug.LogError("LocalizationDataSO is null!");
            return;
        }

        IsInitialized = false;

        localizedText.Clear();

        // 🔤 LOAD XML (TEXT)
        string fileName = data.languageName.ToString().ToLower();
        string path = LocalizationPaths.GetResourcesPath(fileName);
        TextAsset xmlFile = Resources.Load<TextAsset>(path);

        if (xmlFile == null)
        {
            Debug.LogError($"Missing localization file: {fileName}");
            return;
        }

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlFile.text);

        XmlNodeList nodes = xmlDoc.SelectNodes("Localization/Entry");

        foreach (XmlNode node in nodes)
        {
            string key = node.Attributes["key"].Value;
            string value = node.Attributes["value"].Value;

            localizedText[key] = value;
        }

        // 🔥 APPLY FONT FROM SO
        CurrentFontStyle = data.font;

        // ✅ VALIDATE KEYS
        foreach (LocalizationKey key in System.Enum.GetValues(typeof(LocalizationKey)))
        {
            string keyString = key.ToString();

            if (!localizedText.ContainsKey(keyString))
            {
                Debug.LogError($"Missing key in XML: {keyString}");
            }
        }

        IsInitialized = true;

        OnLanguageChanged?.Invoke();
    }


    public string GetText(LocalizationKey key)
    {
        string keyString = key.ToString();

        if (localizedText.TryGetValue(keyString, out string value))
            return value;

        Debug.LogError($"Missing localization key: {keyString}");
        return $"[{keyString}]";
    }
}