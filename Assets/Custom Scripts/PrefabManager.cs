using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    [System.Serializable]
    public class LocationPrefab
    {
        public string locationName;
        public GameObject prefab;
    }

    [SerializeField] private LocationPrefab[] locationPrefabs;

    void Start()
    {
        // Get the selected location from SceneDataManager
        string selectedLocation = SceneDataManager.GetSelectedSpot();
        Debug.Log($"[PREFAB] Selected location: {selectedLocation}");

        // Hide all prefabs first
        foreach (var item in locationPrefabs)
        {
            if (item.prefab != null)
            {
                item.prefab.SetActive(false);
            }
        }

        // Show only the selected prefab
        var selectedItem = System.Array.Find(locationPrefabs, 
            item => item.locationName.Equals(selectedLocation, System.StringComparison.OrdinalIgnoreCase));

        if (selectedItem != null && selectedItem.prefab != null)
        {
            selectedItem.prefab.SetActive(true);
            Debug.Log($"[PREFAB] Activated prefab for: {selectedLocation}");
        }
        else
        {
            Debug.LogError($"[PREFAB] No prefab found for location: {selectedLocation}");
        }
    }

}