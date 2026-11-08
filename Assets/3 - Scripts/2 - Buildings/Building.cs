using UnityEngine.AI;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] public UnitSO[] UnitOSList;
    [SerializeField] public FactionManager.Factions faction;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform destinationPoint;
    [SerializeField] internal ResourceManager.ResourceTypes CollectionType;
    [SerializeField] public Animator collectionAnimator;

    public bool showMenu;
    public Transform SpawnPoint => spawnPoint;
    public Transform DestinationPoint => destinationPoint;

    void Start()
    {
        // Manager
        BuildManager.Instance.AddBuildingToList(faction, gameObject);
    }

    public void OnDestroy()
    {
        BuildManager.Instance.RemoveBuildingToList(faction, gameObject);
    }

    public void SetBuildingUI() {
        if (!showMenu)
        {
            Debug.Log("[Building] Show menu set false");
            return;
        }
        if (UnitOSList.Length == 0)
        {
            Debug.Log("[Building] Empty UnitOSList");
            return;
        }
        ButtonManager.Instance.SetUi(this);
        Debug.Log("[Building] Setting UI");
    }

    // Need to redo
    internal void StartUnitProduction(UnitSO unit)
    {
        if (unit.prefab == null)
        {
            Debug.LogWarning($"No unit prefab assigned to {gameObject.name}");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning($"No spawn point assigned to {gameObject.name}");
            return;
        }

        ResourceManager.Instance.SubtractResource(faction, unit.costType, unit.cost);
        GameObject newUnit = Instantiate(
            unit.prefab,
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
        ResourceManager.Instance.AddResource(faction, resource, 10);
        TriggerCollection();
    }

    internal void TriggerCollection()
    {
        collectionAnimator.SetTrigger("Collecting");
    }
}
