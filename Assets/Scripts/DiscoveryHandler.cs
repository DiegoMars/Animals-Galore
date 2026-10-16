using UnityEngine;

public class DiscoveryHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Discovery mesh hit: {other.name}");
        Resource resource = other.GetComponentInParent<Resource>();

        if (resource == null)
        {
            return;
        }

        ResourceManager.Instance.DiscoverResource(resource);
    }
}