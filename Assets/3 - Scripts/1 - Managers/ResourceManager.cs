using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [Header("General")]
    public static ResourceManager Instance {get; set;}
    public List<GameObject> AllResources = new List<GameObject>();
    public List<GameObject> DiscoveredResources = new List<GameObject>();
    public List<GameObject> CollectedResources = new List<GameObject>();

    [Header("Collected Resources")]
    [SerializeField] private Dictionary<FactionManager.Factions, FactionResourceState> factionResources
        = new Dictionary<FactionManager.Factions, FactionResourceState>();

    [Serializable]
    public class FactionResourceState
    {
        public int wood;
        public int food;
        public int stone;
        public int tech;
    }
    public enum ResourceTypes
    {
        NONE,
        ALL,
        WOOD,
        FOOD,
        STONE,
        TECH
    }

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

    public void InitFactionResourceState(FactionManager.Factions faction)
    {
        if (!factionResources.TryAdd(faction, new FactionResourceState()))
            Debug.Log($"[Resource Manager] Failed to add {faction} states");
    }

    private FactionResourceState GetFactionResources(FactionManager.Factions faction)
    {
        if (factionResources.TryGetValue(faction, out FactionResourceState state))
        {
            return state;
        }
        Debug.LogError($"[Resource Manager] No resource state exists for {faction}");
        return null;
    }

    internal void DiscoverResource(ResourceProducer resource)
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

    internal bool IsResourceDiscovered(ResourceProducer resource)
    {
        foreach (GameObject discoveredResourceObject in DiscoveredResources)
        {
            ResourceProducer discoveredResource = discoveredResourceObject.GetComponentInParent<ResourceProducer>();
            if (discoveredResource == resource)
            {
                Debug.Log($"[Resource Manager] {resource} is discovered");
                return true;
            }
        }
        Debug.Log($"[Resource Manager] {resource} is not discovered");
        return false;
    }

    internal int ResourceQuantity(FactionManager.Factions faction, ResourceTypes resource)
    {
        FactionResourceState state = GetFactionResources(faction);
        switch (resource)
        {
            case ResourceTypes.WOOD: 
                return state.wood;
            case ResourceTypes.FOOD: 
                return state.food;
            case ResourceTypes.STONE: 
                return state.stone;
            case ResourceTypes.TECH: 
                return state.tech;
            default:
                Debug.Log($"[Resource Manager] {resource} is not valid");
                return -1;
        }
    }

    internal void AddResource(FactionManager.Factions faction, ResourceTypes resource, int quantity)
    {
        FactionResourceState state = GetFactionResources(faction);
        switch (resource)
        {
            case ResourceTypes.WOOD: 
                state.wood += quantity;
                break;
            case ResourceTypes.FOOD: 
                state.food += quantity;
                break;
            case ResourceTypes.STONE: 
                state.stone += quantity;
                break;
            case ResourceTypes.TECH: 
                state.tech += quantity;
                break;
            default:
                Debug.Log($"[Resource Manager] {resource} is not valid");
                break;
        }
        if (faction == FactionManager.Instance.PlayerFaction)
            TopBarManager.Instance.UpdateBar(state);
    }

    internal void SubtractResource(FactionManager.Factions faction, ResourceTypes resource, int quantity)
    {
        FactionResourceState state = GetFactionResources(faction);
        switch (resource)
        {
            case ResourceTypes.WOOD: 
                state.wood -= quantity;
                break;
            case ResourceTypes.FOOD: 
                state.food -= quantity;
                break;
            case ResourceTypes.STONE: 
                state.stone -= quantity;
                break;
            case ResourceTypes.TECH: 
                state.tech -= quantity;
                break;
            default:
                Debug.Log($"[Resource Manager] {resource} is not valid");
                break;
        }
        if (faction == FactionManager.Instance.PlayerFaction)
            TopBarManager.Instance.UpdateBar(state);
    }
}