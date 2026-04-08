public static class LocalizationPaths
{
    public const string RESOURCES_LOCALIZATION_PATH = "Localization";
    public const string RESOURCES_FULL_PATH = "Assets/Resources/Localization";
    public const string XML_EXTENSION = ".xml";

    public static string GetFullPath(string fileName)
    {
        return System.IO.Path.Combine(
            RESOURCES_FULL_PATH,
            fileName.ToLower() + XML_EXTENSION
        );
    }

    public static string GetResourcesPath(string fileName)
    {
        return $"{RESOURCES_LOCALIZATION_PATH}/{fileName.ToLower()}";
    }
}