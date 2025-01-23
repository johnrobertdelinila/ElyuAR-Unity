// SceneDataManager.cs
public static class SceneDataManager
{
    private static string selectedSpotName;

    public static void SetSelectedSpot(string spotName)
    {
        selectedSpotName = spotName;
    }

    public static string GetSelectedSpot()
    {
        return selectedSpotName;
    }
}