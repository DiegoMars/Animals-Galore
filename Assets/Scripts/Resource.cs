using System;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] private bool discovered;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        discovered = false;
        ResourceManager.Instance.AllResources.Add(gameObject);
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
}
