using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [Header("General")]
    public static ResourceManager Instance {get; set;}
    public List<GameObject> AllResources = new List<GameObject>();
    [SerializeField] private Dictionary<FactionManager.Factions, List<GameObject>> factionDiscoveredResources
        = new Dictionary<FactionManager.Factions, List<GameObject>>();
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

        foreach (FactionManager.Factions faction in
                Enum.GetValues(typeof(FactionManager.Factions)))
        {
            factionResources.Add(faction, new FactionResourceState());
            factionDiscoveredResources.Add(faction, new List<GameObject>());
        }
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

    public List<GameObject> GetFactionDiscoveredResourcesList(FactionManager.Factions faction)
    {
        if (factionDiscoveredResources.TryGetValue(faction, out List<GameObject> resourceList))
            return resourceList;
        Debug.LogError($"[Resource Manager] No building list for {faction}");
        return null;
    }

    internal void DiscoverResource(FactionManager.Factions faction, ResourceProducer resource)
    {
        List<GameObject> DiscoveredRes = GetFactionDiscoveredResourcesList(faction);
        GameObject resourceParent = resource.gameObject;
        if (DiscoveredRes.Contains(resourceParent))
            return;

        DiscoveredRes.Add(resourceParent);
        if (FactionManager.Instance.IsPlayer(faction))
            resource.SetDiscovered(true);
    }

    internal bool IsResourceDiscovered(FactionManager.Factions faction, ResourceProducer resource)
    {
        List<GameObject> DiscoveredResources = GetFactionDiscoveredResourcesList(faction);
        GameObject resourceParent = resource.gameObject;
        bool value = DiscoveredResources.Contains(resourceParent);
        Debug.Log($"[Resource Manager] {faction} discovered {resource}: {value}");
        return value;
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