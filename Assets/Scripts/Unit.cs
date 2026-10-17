using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("General")]
    public bool Harvestor = false;
    public ResourceManager.ResourceTypes CarryingResource = ResourceManager.ResourceTypes.NONE;

    [Header("For Reference")]
    public bool Harvesting = false;
    public ResourceProducer ResourceTarget;
    public Building Collector;

    private UnitMovement UnitMover;

    void Start()
    {
        UnitSelectionManager.Instance.allUnitsList.Add(gameObject);
        TopBarManager.Instance.UpdatePopulation();
        UnitMover = this.GetComponentInParent<UnitMovement>();
    }

    // Update is called once per frame
    public void OnDestroy()
    {
        UnitSelectionManager.Instance.allUnitsList.Remove(gameObject);
    }

    internal void StartCollection(ResourceProducer resource)
    {
        ResourceTarget = resource;
        ResourceManager.ResourceTypes resourceType = ResourceTarget.ResourceType;
        Vector3 targetPos = ResourceTarget.transform.position;
        Collector = BuildManager.Instance.FindNearestCollector(targetPos, resourceType);
        Debug.Log($"[Unit] Collector: {Collector}");
        Collector.TriggerCollection();
        Harvesting = true;
    
        // Need implement to deposit the resource currently being carried
        // before harvesting another
        if (CarryingResource == ResourceManager.ResourceTypes.NONE)
        {
            HarvestResource();
        }
        else
        {
            DepositResource();
        }
    }

    internal void DepositResource()
    {
        UnitMover.agent.SetDestination(Collector.transform.position);
        Debug.Log("[Unit] Depositing resource");
    }

    internal void HarvestResource()
    {
        UnitMover.agent.SetDestination(ResourceTarget.transform.position);
        Debug.Log("[Unit] Harvesting resource");
    }
}