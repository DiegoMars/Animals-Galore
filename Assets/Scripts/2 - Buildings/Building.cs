using TMPro;
using UnityEngine.AI;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private ResourceManager.ResourceTypes UnitCostType;
    [SerializeField] private int UnitCostValue;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform destinationPoint;
    [SerializeField] internal ResourceManager.ResourceTypes CollectionType;
    [SerializeField] public Animator collectionAnimator;

    public bool showMenu;
    public Transform SpawnPoint => spawnPoint;
    public Transform DestinationPoint => destinationPoint;

    void Start()
    {
        BuildManager.Instance.allBuildingsList.Add(gameObject);
    }

    public void OnDestroy()
    {
        BuildManager.Instance.allBuildingsList.Remove(gameObject);
    }

    public void SetBuildingUI() {
        if (showMenu) {
            ButtonManager.Instance.SetUi(unitPrefab.name,
                                         UnitCostType,
                                         UnitCostValue);
        }
    }

    internal void StartUnitProduction()
    {
        if (unitPrefab == null)
        {
            Debug.LogWarning($"No unit prefab assigned to {gameObject.name}");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning($"No spawn point assigned to {gameObject.name}");
            return;
        }

        ResourceManager.Instance.SubtractResource(UnitCostType, UnitCostValue);
        GameObject newUnit = Instantiate(
            unitPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );
        newUnit.GetComponent<NavMeshAgent>().SetDestination(destinationPoint.position);
        Debug.Log($"Created {newUnit.name} at {gameObject.name}");
    }

    internal void MoveDestination(Vector3 point)
    {
        destinationPoint.position = point;
    }

    internal void CollectResource(ResourceManager.ResourceTypes resource)
    {
        Debug.Log($"[Building] Collected {resource}");
        ResourceManager.Instance.AddResource(resource, 10);
        TriggerCollection();
    }

    internal void TriggerCollection()
    {
        collectionAnimator.SetTrigger("Collecting");
    }
}
