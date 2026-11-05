using System;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [Header("General")]
    public UnitSO unitInfo;
    public FactionManager.Factions Faction;
    public int CurrentHealth;
    public bool Harvestor = false;

    [Header("Harvestor")]
    public bool Harvesting = false;
    public ResourceManager.ResourceTypes CarryingResource = ResourceManager.ResourceTypes.NONE;
    public ResourceProducer ResourceTarget;
    public Building Collector;

    // [Header("Troop")]

    private UnitMovement UnitMover;
    private NavMeshAgent UnitMeshAgent;

    void Start()
    {
        // Manager Stuff
        SelectionManager.Instance.allUnitsList.Add(gameObject);
        TopBarManager.Instance.UpdatePopulation();
        UnitMover = this.GetComponentInParent<UnitMovement>();
        UnitMeshAgent = this.GetComponentInParent<NavMeshAgent>();

        // Setting stuff
        Faction = unitInfo.Faction;
        Harvestor = unitInfo.Harvestor;
        CurrentHealth = unitInfo.MaxHealth;
        UnitMeshAgent.speed = unitInfo.MovementSpeed;
    }

    // Update is called once per frame
    public void OnDestroy()
    {
        SelectionManager.Instance.allUnitsList.Remove(gameObject);
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