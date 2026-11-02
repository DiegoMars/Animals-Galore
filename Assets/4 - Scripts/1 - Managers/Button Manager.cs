using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("General")]
    public static ButtonManager Instance {get; set;}
    public GameObject BuildingMenu;
    public Button ActualButton;
    public TextMeshProUGUI UnitName;
    public TextMeshProUGUI UnitCost;
    public ResourceManager.ResourceTypes UnitCostType;
    public int UnitCostValue;
    public Color32 ColorActive;
    public Color32 ColorInactive;

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

    internal void Start()
    {
        ActualButton = BuildingMenu.GetComponentInChildren<Button>();
        foreach (TextMeshProUGUI text in BuildingMenu.GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (text.gameObject.name == "UnitName")
            {
                Debug.Log("[Button Manager] Found UnitName text mesh");
                UnitName = text;
            }
            if (text.gameObject.name == "UnitCost")
            {
                Debug.Log("[Button Manager] Found UnitCost text mesh");
                UnitCost = text;
            }
        }
        if (UnitName && UnitCost)
            Debug.Log("[Button Manager] Found both text meshes");
        else
            Debug.Log("[Button Manager] 1 or more text meshes not found");
    }

    public void Update()
    {
        if (ResourceManager.Instance.ResourceQuantity(UnitCostType) >= UnitCostValue)
        {
            EnableButton();
        }
        else
        {
            DisableButton();
        }
    }

    internal void SetUi(String name, ResourceManager.ResourceTypes type, int cost)
    {
        UnitName.text = name;
        UnitCost.text = $"{cost.ToString()} {type.ToString()}";
        BuildingMenu.SetActive(true);
        UnitCostType = type;
        UnitCostValue = cost;
    }

    private void EnableButton()
    {
       ActualButton.enabled = true; 
       ActualButton.image.color = ColorActive;
    }

    private void DisableButton()
    {
       ActualButton.enabled = false; 
       ActualButton.image.color = ColorInactive;
    }
}