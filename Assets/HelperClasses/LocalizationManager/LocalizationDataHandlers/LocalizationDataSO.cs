using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationData", menuName = "Localization/Language Data")]
public class LocalizationDataSO : ScriptableObject
{
    public Language languageName;
    public TMP_FontAsset font;
    public List<LocalizationEntry> entries = new();
}


[Serializable]
public class LocalizationEntry
{
    public LocalizationKey key;
    [TextArea] public string value;
}


public enum Language
{
    English,
    Spanish
}

public enum LocalizationKey
{
    MenuText
}
