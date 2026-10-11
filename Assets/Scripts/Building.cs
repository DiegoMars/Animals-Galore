using TMPro;
using UnityEngine.AI;
using UnityEngine;
using System;

public class Building : MonoBehaviour
{
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform destinationPoint;
    public bool showMenu;
    public Transform SpawnPoint => spawnPoint;
    public Transform DestinationPoint => destinationPoint;

    public void SetBuildingUI(GameObject menu) {
        if (showMenu) {
            menu.SetActive(true);
            menu.GetComponentInChildren<TextMeshProUGUI>().text = unitPrefab.name;
        }
    }

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
        newUnit.GetComponent<NavMeshAgent>().SetDestination(destinationPoint.position);
        Debug.Log($"Created {newUnit.name} at {gameObject.name}");
    }

    internal void MoveDestination(Vector3 point)
    {
        destinationPoint.position = point;
    }
}
