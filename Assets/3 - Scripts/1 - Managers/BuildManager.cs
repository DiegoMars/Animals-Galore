using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BuildManager : MonoBehaviour
{
    [Header("General")]
    public static BuildManager Instance {get; set;}
    public bool isPlacing;
    private LayerMask Ground;
    private LayerMask Clickable;
    [SerializeField] private Dictionary<FactionManager.Factions, List<GameObject>> factionBuildingsList
        = new Dictionary<FactionManager.Factions, List<GameObject>>();

    [Header("Buildings")]
    public BuildingSO[] BuildingSOs;
    [Header("Materials")]
    public Material Ghost;
    public Material WrongGhost;

    private FactionManager.Factions playerFaction;
    private GameObject FollowObject;
    private BuildingSO selectedBuildingSO;
    private Camera cam;

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

        foreach (FactionManager.Factions faction in
                Enum.GetValues(typeof(FactionManager.Factions)))
        {
            factionBuildingsList.Add(faction, new List<GameObject>());
        }

        Building[] initialBuildings = FindObjectsByType<Building>();

        foreach (Building building in initialBuildings)
        {
            building.Initialize();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerFaction = FactionManager.Instance.PlayerFaction;
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
                    PlaceBuilding();
                }
            }
            else
            {
                FollowObject.SetActive(false);
            }
        }
    }

    private List<GameObject> GetFactionBuildingList(FactionManager.Factions faction)
    {
        if (factionBuildingsList.TryGetValue(faction, out List<GameObject> buildingList))
            return buildingList;
        Debug.LogError($"[Build Manager] No building list for {faction}");
        return null;
    }

    public void AddBuildingToList(FactionManager.Factions faction, GameObject building)
    {
        List<GameObject> buildingList = GetFactionBuildingList(faction);
        if (buildingList != null)
        {
            buildingList.Add(building);
            return;
        }
        Debug.Log("[Build Manager] No faction building list to add to");
    }

    public void RemoveBuildingFromList(FactionManager.Factions faction, GameObject building)
    {
        List<GameObject> buildingList = GetFactionBuildingList(faction);
        if (buildingList != null)
        {
            buildingList.Remove(building);
            return;
        }
        Debug.Log("[Build Manager] No faction building list to remove from");
    }

    public void selectBuildingPrefab(BuildingSO building)
    {
        if (ResourceManager.Instance.ResourceQuantity(FactionManager.Instance.PlayerFaction,
            building.CostType) < building.Cost)
        {
            Debug.Log($"[Build Manager] Can't afford {building.BuildingName}");
            return;
        }
        
        selectedBuildingSO = building;

        // Remove the previous preview
        if (FollowObject != null)
        {
            Destroy(FollowObject);
        }

        // Create a preview instance
        FollowObject = Instantiate(building.Prefab);
        FollowObject.name = building.Prefab.name + "_Preview";

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
        FollowObject.tag = "Untagged";

        isPlacing = true;
    }

    private void PlaceBuilding()
    {
        ResourceManager.Instance.SubtractResource(FactionManager.Instance.PlayerFaction,
                                                  selectedBuildingSO.CostType,
                                                  selectedBuildingSO.Cost);
        Building placedBuilding = Instantiate(selectedBuildingSO.Prefab,
                                              FollowObject.transform.position,
                                              FollowObject.transform.rotation)
                                              .GetComponentInParent<Building>();
        placedBuilding.Initialize(playerFaction);
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

    // Will enventually be expanded to find only a specific type of building
    internal Building FindNearestCollector(FactionManager.Factions faction, Vector3 pos,
                                           ResourceManager.ResourceTypes resourceType)
    {
        float closestDistance = Mathf.Infinity;
        Building closestCollector = null;
        List<GameObject> buildingList = GetFactionBuildingList(faction);
        foreach (GameObject building in buildingList)
        {
            Building build = building.GetComponentInParent<Building>();
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
                closestDistance = distanceSq;
            }
        }
        return closestCollector;
    }
}
