using UnityEngine;

public class DiscoveryHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[Discovery Handler] Discovery mesh hit: {other.name}");
        ResourceProducer resource = other.GetComponentInParent<ResourceProducer>();

        if (resource == null)
        {
            return;
        }

        ResourceManager.Instance.DiscoverResource(resource);
    }
}
