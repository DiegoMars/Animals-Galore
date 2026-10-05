using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

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
    public GameObject groundMarker;

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
                    DeselectAll();
                    selectedBuilding = building;
                    buildingMenu.SetActive(true);
                    Debug.Log($"Selected Building: {building.name}");
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
                    buildingMenu.SetActive(false);
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
        if (Input.GetMouseButtonDown(1) && unitsSelected.Count > 0)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                groundMarker.transform.position = hit.point;
                groundMarker.SetActive(false);
                groundMarker.SetActive(true);
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

        buildingMenu.SetActive(false);
        groundMarker.SetActive(false);
        unitsSelected.Clear();
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

    private void SelectUnit(GameObject unit, bool value)
    {
        if (value)
        {
            unitsSelected.Add(unit);
            EnableMovement(unit, true);
            TriggerIndicator(unit, true);
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

    private void TriggerIndicator(GameObject thing, bool isVisible)
    {
        thing.transform.Find("SelectionIndicator").gameObject.SetActive(isVisible);
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