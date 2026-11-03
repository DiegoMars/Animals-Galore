using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [Header("General")]
    public static ResourceManager Instance {get; set;}
    public List<GameObject> AllResources = new List<GameObject>();
    public List<GameObject> DiscoveredResources = new List<GameObject>();
    public List<GameObject> CollectedResources = new List<GameObject>();

    [Header("Collected Resources")]
    public TopBarManager TopBar;
    public int WoodQuantity;
    public int FoodQuantity;
    public int StoneQuantity;
    public int TechPointsQuantity;

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

    void Start()
    {
        WoodQuantity = 0;
        FoodQuantity = 0;
        StoneQuantity = 0;
        TechPointsQuantity = 0;
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

    internal int ResourceQuantity(ResourceTypes resource)
    {
        switch (resource)
        {
            case ResourceTypes.WOOD: 
                return WoodQuantity;
            case ResourceTypes.FOOD: 
                return FoodQuantity;
            case ResourceTypes.STONE: 
                return StoneQuantity;
            case ResourceTypes.TECH: 
                return TechPointsQuantity;
            default:
                Debug.Log($"[Resource Manager] {resource} is not valid");
                return -1;
        }
    }

    internal void AddResource(ResourceTypes resource, int quantity)
    {
        switch (resource)
        {
            case ResourceTypes.WOOD: 
                WoodQuantity += quantity;
                TopBar.SetWood(WoodQuantity.ToString());
                break;
            case ResourceTypes.FOOD: 
                FoodQuantity += quantity;
                TopBar.SetFood(FoodQuantity.ToString());
                break;
            case ResourceTypes.STONE: 
                StoneQuantity += quantity;
                TopBar.SetStone(StoneQuantity.ToString());
                break;
            case ResourceTypes.TECH: 
                TechPointsQuantity += quantity;
                TopBar.SetTech(TechPointsQuantity.ToString());
                break;
            default:
                Debug.Log($"[Resource Manager] {resource} is not valid");
                break;
        }
    }

    internal void SubtractResource(ResourceTypes resource, int quantity)
    {
        switch (resource)
        {
            case ResourceTypes.WOOD: 
                WoodQuantity -= quantity;
                TopBar.SetWood(WoodQuantity.ToString());
                break;
            case ResourceTypes.FOOD: 
                FoodQuantity -= quantity;
                TopBar.SetFood(FoodQuantity.ToString());
                break;
            case ResourceTypes.STONE: 
                StoneQuantity -= quantity;
                TopBar.SetStone(StoneQuantity.ToString());
                break;
            case ResourceTypes.TECH: 
                TechPointsQuantity -= quantity;
                TopBar.SetTech(TechPointsQuantity.ToString());
                break;
            default:
                Debug.Log($"[Resource Manager] {resource} is not valid");
                break;
        }
    }
}