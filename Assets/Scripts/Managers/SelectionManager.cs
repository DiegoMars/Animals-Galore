using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using UnityEngine;
using System;

public class UnitSelectionManager : MonoBehaviour
{
    [Header("General")]
    public static UnitSelectionManager Instance {get; set;}
    public List<GameObject> allUnitsList = new List<GameObject>();
    public List<GameObject> unitsSelected = new List<GameObject>();

    [Header("Building UI")]
    public GameObject buildingMenu;

    [Header("Layer Masks")]
    public LayerMask clickable;
    public LayerMask ground;

    [Header("Markers")]
    public GameObject unitMarker;
    public GameObject buildingMarker;

    private Building selectedBuilding;
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
    }

    private void Start()
    {
        cam = Camera.main;
        Debug.Log($"Selected Building value: {selectedBuilding}");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) &&
            EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickable))
            {
                Building building = hit.collider.GetComponentInParent<Building>();
                if (building != null)
                {
                    SelectBuilding(building, true);
                }
                else if (hit.collider.CompareTag("Unit"))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        MultiSelect(hit.collider.gameObject);
                    }
                    else
                    {
                        SelectByClicking(hit.collider.gameObject);
                    }
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftShift) == false)
                {
                    DeselectAll();
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                if (unitsSelected.Count > 0) {
                    MoveUnitMarker(hit.point);
                }
                else if (selectedBuilding) {
                    selectedBuilding.MoveDestination(hit.point);
                    MoveBuildingMarker(hit.point);
                }
            }
        }
    }

    public void DeselectAll()
    {
        foreach (var unit in unitsSelected)
        {
            EnableMovement(unit, false);
            TriggerIndicator(unit, false);
        }

        if (selectedBuilding)
        {
            SelectBuilding(selectedBuilding, false);
        }

        buildingMenu.SetActive(false);
        unitMarker.SetActive(false);
        buildingMarker.SetActive(false);
        unitsSelected.Clear();
    }

    private void MoveUnitMarker(Vector3 pos) {
        unitMarker.SetActive(false);
        unitMarker.transform.position = pos;
        unitMarker.SetActive(true);
    }

    private void MultiSelect(GameObject unit)
    {
        if (unitsSelected.Contains(unit) == false)
        {
            SelectUnit(unit, true);
        }
        else
        {
            SelectUnit(unit, false);
        }
    }

    private void SelectByClicking(GameObject unit)
    {
        DeselectAll();

        SelectUnit(unit, true);
    }

    internal void DragSelect(GameObject unit)
    {
        if (unitsSelected.Contains(unit) == false)
        {
            SelectUnit(unit, true);
        }
    }

    private void TriggerIndicator(GameObject thing, bool isVisible)
    {
        thing.transform.Find("SelectionIndicator").gameObject.SetActive(isVisible);
    }
    private void TriggerIndicator(Building thing, bool isVisible)
    {
        thing.transform.Find("SelectionIndicator").gameObject.SetActive(isVisible);
    }

    private void SelectUnit(GameObject unit, bool value)
    {
        if (value)
        {
            unitsSelected.Add(unit);
            EnableMovement(unit, true);
            TriggerIndicator(unit, true);

            // Show what the destination is
            NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();
            Vector3 destination = agent.destination;
            float remainingDistance = agent.remainingDistance;
            if (remainingDistance >= 2) {
                MoveUnitMarker(destination);
            }
        }
        else
        {
            unitsSelected.Remove(unit);
            EnableMovement(unit, false);
            TriggerIndicator(unit, false);
        }
    }

    private void EnableMovement(GameObject unit, bool shouldMove)
    {
        unit.GetComponent<UnitMovement>().enabled = shouldMove;
    }

    private void MoveBuildingMarker(Vector3 pos)
    {
        buildingMarker.SetActive(false);
        buildingMarker.transform.position = pos;
        buildingMarker.SetActive(true);
    }

    private void SelectBuilding(Building building, bool value)
    {
        if (value)
        {
            DeselectAll();
            // buildingsSelected.Add(building);
            TriggerIndicator(building, true);
            selectedBuilding = building;
            selectedBuilding.SetBuildingUI(buildingMenu);

            Vector3 buildingPos = building.transform.position;
            Vector3 destinationPos = building.transform.Find("Destination").gameObject.transform.position;
            if (Vector3.Distance(buildingPos, destinationPos) > 2f) {
                MoveBuildingMarker(destinationPos);
            }
        }
        else
        {
            // buildingsSelected.Remove(building);
            TriggerIndicator(building, false);
            buildingMenu.SetActive(false);
            selectedBuilding = null;
        }
    }

    public void MakeUnitButton()
    {
        if (selectedBuilding == null)
        {
            Debug.LogWarning("No building is currently selected");
            return;
        }
        selectedBuilding.StartUnitProduction();
    }
}
