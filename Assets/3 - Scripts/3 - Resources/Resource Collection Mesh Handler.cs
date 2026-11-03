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
        if (other.transform.parent)
            Debug.Log($"[ResourceCollectionMesh] {transform.parent.name}'s " +
                    $"Discovery mesh hit: {other.transform.parent.name}");
        else
            Debug.Log($"[ResourceCollectionMesh] {transform.parent.name}'s " +
                    $"Discovery mesh hit: {other.name}");
        Unit harvestor = other.GetComponentInParent<Unit>();
        if (harvestor == null)
        {
            Debug.Log($"[CollectionTriggerHandler] Not a harvestor");
            return;
        }
        
        if (harvestor.Harvestor && harvestor.Harvesting)
        {
            if (harvestor.CarryingResource == ResourceManager.ResourceTypes.NONE)
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
            else
            {
                Debug.Log($"[ResourceCollectionMesh] Already carrying resource");
                harvestor.DepositResource();
            }
        }
    }
}
