using UnityEngine;

public class DiscoveryHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent)
            Debug.Log($"[Discovery Handler] {transform.parent.name}'s " +
                    $"Discovery mesh hit: {other.transform.parent.name}");
        else
            Debug.Log($"[Discovery Handler] {transform.parent.name}'s " +
                    $"Discovery mesh hit: {other.name}");
        ResourceProducer resource = other.GetComponentInParent<ResourceProducer>();

        if (resource == null)
        {
            return;
        }

        ResourceManager.Instance.DiscoverResource(resource);
    }
}
