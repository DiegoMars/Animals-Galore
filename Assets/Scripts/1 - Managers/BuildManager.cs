using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BuildManager : MonoBehaviour
{
    [Header("General")]
    public static BuildManager Instance {get; set;}
    public GameObject PrefabObject;
    public bool isPlacing;
    private LayerMask Ground;
    private LayerMask Clickable;
    public List<GameObject> allBuildingsList = new List<GameObject>();

    [Header("Prefabs")]
    public GameObject FarmPrefab; // 0
    public GameObject BarracksPrefab; // 1
    public GameObject HousePrefab; //2

    [Header("Materials")]
    public Material Ghost;
    public Material WrongGhost;

    private GameObject FollowObject;
    private Camera cam;
    private Dictionary<KeyCode, int> buildingKeys = new Dictionary<KeyCode, int>
    {
        {KeyCode.Q, 0}, // Farm
        {KeyCode.A, 1}, // Barracks
        {KeyCode.W, 2}, // House
    };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
        isPlacing = false;
        FollowObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        FollowObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            FollowObject.SetActive(false);
            isPlacing = false;
        }
        if (Input.anyKeyDown) 
        {
            foreach (KeyCode key in buildingKeys.Keys)
            {
                if (Input.GetKeyDown(key))
                {
                    OnBuildingSelection(buildingKeys[key]);
                    break; 
                }
            }
        }
        if (isPlacing)
        {
            RaycastHit hitInfo = new RaycastHit();
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out hitInfo);
            if (hit)
            {
                bool canPlace = hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Ground");

                FollowObject.SetActive(true);
                FollowObject.transform.position = hitInfo.point;

                Renderer[] renderers = FollowObject.GetComponentsInChildren<Renderer>(true);

                foreach (Renderer renderer in renderers)
                {
                    renderer.sharedMaterial = canPlace ? Ghost : WrongGhost;
                }

                if (canPlace && Input.GetMouseButtonDown(0))
                {
                    GameObject placedBuilding = Instantiate(PrefabObject,
                                                            FollowObject.transform.position,
                                                            FollowObject.transform.rotation);
                    NavMeshObstacle obstacle =
                        placedBuilding.transform
                        .Find("Mesh")
                        ?.GetComponent<NavMeshObstacle>();

                    if (obstacle != null)
                    {
                        obstacle.enabled = true;
                    }
                    else
                    {
                        Debug.LogWarning(
                            $"No NavMeshObstacle found on the Mesh child of {placedBuilding.name}"
                        );
                    }
                    FollowObject.SetActive(false);
                    isPlacing = false;
                }
            }
            else
            {
                FollowObject.SetActive(false);
            }
        }
    }

    public void OnBuildingSelection(int index)
    {
        switch (index)
        {
            case 0: 
                Debug.Log($"[Build Manager] You selected {index}, the Farm");
                selectBuildingPrefab(FarmPrefab);
                break;
            case 1: 
                Debug.Log($"[Build Manager] You selected {index}, the Barracks");
                selectBuildingPrefab(BarracksPrefab);
                break;
            case 2: 
                Debug.Log($"[Build Manager] You selected {index}, the House");
                selectBuildingPrefab(HousePrefab);
                break;
            default: 
                Debug.Log($"[Build Manager] Error with building selection");
                break;
        }
    }

    private void selectBuildingPrefab(GameObject buildingPrefab)
    {
        PrefabObject = buildingPrefab;

        // Remove the previous preview
        if (FollowObject != null)
        {
            Destroy(FollowObject);
        }

        // Create a preview instance
        FollowObject = Instantiate(PrefabObject);
        FollowObject.name = PrefabObject.name + "_Preview";

        // Apply the ghost material to the root and all children
        Renderer[] renderers =
            FollowObject.GetComponentsInChildren<Renderer>(true);

        foreach (Renderer renderer in renderers)
        {
            renderer.sharedMaterial = Ghost;
        }

        // Disable colliders so the preview does not interfere with raycasts
        Collider[] colliders =
            FollowObject.GetComponentsInChildren<Collider>(true);

        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
        FollowObject.layer = LayerMask.NameToLayer("TransparentFX");

        isPlacing = true;
    }

    // Will enventually be expanded to find only a specific type of building
    internal Building FindNearestCollector(Vector3 pos, ResourceManager.ResourceTypes resourceType) {
        float closestDistance = Mathf.Infinity;
        Building closestCollector = allBuildingsList[0].GetComponent<Building>();
        foreach (GameObject building in allBuildingsList)
        {
            Building build = building.GetComponent<Building>();
            ResourceManager.ResourceTypes buildingType = build.CollectionType;
            if (buildingType != resourceType && buildingType != ResourceManager.ResourceTypes.ALL)
            {
                continue;
            }
            Vector3 direction = building.transform.position - pos;
            float distanceSq = direction.sqrMagnitude;
            if (distanceSq < closestDistance)
            {
                closestCollector = build;
            }
        }
        return closestCollector;
    }
}
