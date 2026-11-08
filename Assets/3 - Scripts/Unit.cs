using System;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [Header("General")]
    public UnitSO unitInfo;
    public FactionManager.Factions Faction;
    public int CurrentHealth;

    [Header("Harvestor")]
    public bool Harvestor = false;
    public bool Harvesting = false;
    public ResourceManager.ResourceTypes CarryingResource = ResourceManager.ResourceTypes.NONE;
    public ResourceProducer ResourceTarget;
    public Building Collector;

    // [Header("Troop")]

    private bool PlayerControlled;
    private UnitMovement UnitMover;
    private NavMeshAgent UnitAgent;

    void Start()
    {
        PlayerControlled = Faction == FactionManager.Instance.PlayerFaction;
        if (PlayerControlled)
        {
            SelectionManager.Instance.allUnitsList.Add(gameObject);
            TopBarManager.Instance.UpdatePopulation();
        }
    }

    void Awake()
    {
        UnitMover = this.GetComponentInParent<UnitMovement>();
        UnitAgent = this.GetComponentInParent<NavMeshAgent>();

        Faction = unitInfo.Faction;
        Harvestor = unitInfo.Harvestor;
        CurrentHealth = unitInfo.MaxHealth;
        UnitAgent.speed = unitInfo.MovementSpeed;
    }

    // Update is called once per frame
    public void OnDestroy()
    {
        SelectionManager.Instance.allUnitsList.Remove(gameObject);
        SelectionManager.Instance.unitsSelected.Remove(gameObject);
        TopBarManager.Instance.UpdatePopulation();
    }

    internal void StartCollection(ResourceProducer resource)
    {
        ResourceTarget = resource;
        ResourceManager.ResourceTypes resourceType = ResourceTarget.ResourceType;
        Vector3 targetPos = ResourceTarget.transform.position;
        Collector = BuildManager.Instance.FindNearestCollector(Faction, targetPos, resourceType);
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

    public void Attack(Unit Target)
    {
        if (Target == null)
            return;
        Target.TakeDamage(unitInfo.Damage);
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}