using UnityEngine;

public class ResourceCollectionMeshHandler : MonoBehaviour
{
    public ResourceProducer ParentResourceProducer;

    void Start()
    {
        ParentResourceProducer = GetComponentInParent<ResourceProducer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[ResourceCollectionMesh] Mesh hit: {other.name}");
        Unit harvestor = other.GetComponentInParent<Unit>();
        if (harvestor == null)
        {
            Debug.Log($"[CollectionTriggerHandler] Not a harvestor");
            return;
        }
        
        if (harvestor.Harvestor && harvestor.Harvesting)
        {
            Debug.Log($"[ResourceCollectionMesh] Attempting to harvest");
            if (ParentResourceProducer.ConsumeResource())
            {
                harvestor.CarryingResource = ParentResourceProducer.ResourceType;
                harvestor.DepositResource();
            }
            else
            {
                harvestor.Harvesting = false;
            }
        }
    }
}
