#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Collections.Generic;


[CustomEditor(typeof(LocalizationDataSO))]
public class LocalizationDataSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        LocalizationDataSO data = (LocalizationDataSO)target;

        if (GUILayout.Button("Export To XML"))
        {
            ExportToXML(data);
        }

        if (GUILayout.Button("Open Localization Folder"))
        {
            EditorUtility.RevealInFinder(LocalizationPaths.RESOURCES_FULL_PATH);
        }
    }

    private void ExportToXML(LocalizationDataSO data)
    {
        if (data == null)
        {
            Debug.LogError("LocalizationDataSO is null!");
            return;
        }

        // ✅ Use constant path
        string folderPath = LocalizationPaths.RESOURCES_FULL_PATH;

        // ✅ Ensure folder exists
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // ✅ Clean filename
        string fileName = data.languageName.ToString().ToLower();

        string path = Path.Combine(
            folderPath,
            fileName + LocalizationPaths.XML_EXTENSION
        );

        XmlWriterSettings settings = new XmlWriterSettings
        {
            Indent = true
        };

        HashSet<LocalizationKey> usedKeys = new();

        using (XmlWriter writer = XmlWriter.Create(path, settings))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("Localization");

            foreach (var entry in data.entries)
            {
                // 🔥 Duplicate check
                if (!usedKeys.Add(entry.key))
                {
                    Debug.LogError($"Duplicate key: {entry.key}");
                    continue;
                }

                writer.WriteStartElement("Entry");
                writer.WriteAttributeString("key", entry.key.ToString());
                writer.WriteAttributeString("value", entry.value);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        Debug.Log($"✅ Exported XML: {path}");
        AssetDatabase.Refresh();
    }
}
#endif



