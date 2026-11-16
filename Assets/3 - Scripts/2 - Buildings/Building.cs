using UnityEngine.AI;
using UnityEngine;

public class Building : MonoBehaviour
{
    
    [Header("General")]
    [SerializeField] public BuildingSO buidlingSO;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform destinationPoint;
    [SerializeField] public Animator collectionAnimator;
    [Header("BuildingStats")]
    public int health;
    public FactionManager.Factions faction;
    public ResourceManager.ResourceTypes CollectionType;
    private UnitSO[] UnitOSList;

    public bool showMenu;
    public Transform SpawnPoint => spawnPoint;
    public Transform DestinationPoint => destinationPoint;
    private HealthBar healthBar;

    public void OnDestroy()
    {
        BuildManager.Instance.RemoveBuildingFromList(faction, gameObject);
    }

    // For initial buildings to be added
    public void Initialize()
    {
        BuildManager.Instance.AddBuildingToList(faction, gameObject);
        SetStats();
    }
    // For all buildings after the start of the game
    public void Initialize(FactionManager.Factions faction)
    {
        this.faction = faction;
        BuildManager.Instance.AddBuildingToList(faction, gameObject);
        SetStats();
    }

    private void SetStats()
    {
        UnitOSList = buidlingSO.UnitSOList;
        health = buidlingSO.MaxHealth;

        healthBar = this.GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.Initialize(health, buidlingSO.MaxHealth);
        }
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
        ButtonManager.Instance.SetUnitButtonsUi(this);
        Debug.Log("[Building] Setting UI");
    }

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

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (healthBar != null)
        {
            healthBar.UpdateHealth(health, buidlingSO.MaxHealth);
        }
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
