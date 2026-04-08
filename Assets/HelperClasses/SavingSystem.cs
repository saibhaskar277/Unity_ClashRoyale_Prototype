using System.IO;
using UnityEngine;

public static class SavingSystem
{
    private static string GetPath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName + ".json");
    }

    // ✅ SAVE (Create or Replace)
    public static void Save<T>(string fileName, T data)
    {
        string path = GetPath(fileName);

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(path, json);

        Debug.Log($"Saved: {fileName} at {path}");
    }

    // ✅ LOAD
    public static T Load<T>(string fileName)
    {
        string path = GetPath(fileName);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"File not found: {fileName}");
            return default;
        }

        string json = File.ReadAllText(path);

        return JsonUtility.FromJson<T>(json);
    }

    // ✅ EXISTS CHECK
    public static bool Exists(string fileName)
    {
        return File.Exists(GetPath(fileName));
    }

    // ✅ DELETE
    public static void Delete(string fileName)
    {
        string path = GetPath(fileName);

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Deleted: {fileName}");
        }
        else
        {
            Debug.LogWarning($"Delete failed. File not found: {fileName}");
        }
    }
}




public class SavingKeys
{
    public const string PLAYER_DATA = "playerData";
}