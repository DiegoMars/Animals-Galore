using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [Header("General")]
    public static ResourceManager Instance {get; set;}
    public List<GameObject> AllResources = new List<GameObject>();
    public List<GameObject> DiscoveredResources = new List<GameObject>();
    public List<GameObject> CollectedResources = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    internal void DiscoverResource(Resource resource)
    {
        GameObject resourceParent = resource.gameObject;
        if (!DiscoveredResources.Contains(resourceParent))
        {
            DiscoveredResources.Add(resourceParent);
            resource.SetDiscovered(true);
        }
        else
        {
            return;
        }
    }
}
