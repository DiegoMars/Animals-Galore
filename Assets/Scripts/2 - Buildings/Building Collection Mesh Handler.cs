using UnityEngine;

public class BuildingCollectionMeshHandler : MonoBehaviour
{
    public Building ParentBuilding;

    void Start()
    {
        ParentBuilding = GetComponentInParent<Building>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[BuildingCollectionMesh] Mesh hit: {other.name}");
        Unit harvestor = other.GetComponentInParent<Unit>();
        if (harvestor == null)
        {
            Debug.Log($"[BuildingCollectionMesh] Not a harvestor");
            return;
        }
        
        if (harvestor.Harvestor && harvestor.Harvesting)
        {
            Debug.Log($"[BuildingCollectionMesh] Attempting deposit");
            if (harvestor.CarryingResource != ResourceManager.ResourceTypes.NONE)
            {
                ParentBuilding.CollectResource(harvestor.CarryingResource);
                harvestor.CarryingResource = ResourceManager.ResourceTypes.NONE;
                harvestor.HarvestResource();
            }
            else
            {
                Debug.Log($"[BuildingCollectionMesh] Harvestor isn't carrying anything");
            }
        }
    }
}
