using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class ResourceProducer : MonoBehaviour
{
    [SerializeField] private bool discovered;
    [SerializeField] internal ResourceManager.ResourceTypes ResourceType;
    public List<GameObject> allResourcesList = new List<GameObject>();
    public List<GameObject> presentResourcesList = new List<GameObject>();
    public List<GameObject> consumedResourcesList = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        discovered = false;
        ResourceManager.Instance.AllResources.Add(gameObject);
        presentResourcesList = allResourcesList;
    }

    public void OnDestroy()
    {
        ResourceManager.Instance.AllResources.Remove(gameObject);
    }

    internal void SetDiscovered(bool v)
    {
        discovered = v;
        this.transform.Find("DiscoveryMarker").gameObject.SetActive(discovered);
    }
    
    internal bool ConsumeResource()
    {
        Debug.Log("[Resource Producer] Attempting to consume resource");
        if (presentResourcesList.Count != 0)
        {
            Debug.Log("[Resource Producer] Consuming resource");
            GameObject resourceGameObject = presentResourcesList[0];
            presentResourcesList.RemoveAt(0);
            consumedResourcesList.Add(resourceGameObject);
            resourceGameObject.GetComponent<Resource>().TriggerCollection();
            return true;
        }
        else
        {
            Debug.Log("[Resource Producer] Consumed all resources");
            return false;
        }
    }
}