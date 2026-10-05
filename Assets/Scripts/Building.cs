using System;
using Unity.VisualScripting;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Transform spawnPoint;
    public Transform SpawnPoint => spawnPoint;

    internal void StartUnitProduction()
    {
        if (unitPrefab == null)
        {
            Debug.LogWarning($"No unit prefab assigned to {gameObject.name}");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning($"No spawn point assigned to {gameObject.name}");
            return;
        }

        GameObject newUnit = Instantiate(
            unitPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        Debug.Log($"Created {newUnit.name} at {gameObject.name}");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
