using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("General")]
    public static ButtonManager Instance {get; set;}
    public GameObject UnitSelectionMenu;
    public GameObject UnitSelectionButtonPrefab;
    public Color32 ColorActive;
    public Color32 ColorInactive;
    private List<ButtonStruct> activeButtons = new();

    private struct ButtonStruct
    {
        UnitSO unitSOb;
        private GameObject UnitButton;
        private Button ActualButton;
        public Color32 ColorActive;
        public Color32 ColorInactive;
        public ButtonStruct(UnitSO unitOS, GameObject UnitButtonPrefab, Transform Menu,
                            Color32 colorActive, Color32 colorInactive, UnityAction onClick)
        {
            unitSOb = unitOS;
            UnitButton = Instantiate(UnitButtonPrefab, Menu);
            ActualButton = UnitButton.GetComponentInChildren<Button>();
            ColorActive = colorActive;
            ColorInactive = colorInactive;
            
            ActualButton.onClick.AddListener(onClick);
            
            bool nameFlag = true;
            bool unitCostFlag = true;
            foreach (TextMeshProUGUI textObject in
                     UnitButton.GetComponentsInChildren<TextMeshProUGUI>())
            {
                if (textObject.gameObject.name == "UnitName")
                {
                    textObject.text = unitSOb.name;
                    nameFlag = false;
                }
                if (textObject.gameObject.name == "UnitCost")
                {
                    textObject.text = $"{unitSOb.cost.ToString()} {unitSOb.costType.ToString()}";
                    unitCostFlag = false;
                }
            }
            if (nameFlag || unitCostFlag)
            {
                Debug.Log("[Button Manager : Button Struct] 1 or more meshes not found");
            }
            Debug.Log($"[Button Manager : Button Struct] End {unitSOb.name} button initialization");
        }
        
        public void CheckButton()
        {

            bool canAfford = ResourceManager.Instance.ResourceQuantity(unitSOb.costType) >= unitSOb.cost;
            ActualButton.enabled = canAfford; 
            ActualButton.image.color = canAfford ? ColorActive : ColorInactive;
        }
        
        public void DestroyButton()
        {
            Destroy(UnitButton);
        }
    }

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

    public void Update()
    {
        if (activeButtons.Count > 0)
        {
            foreach (ButtonStruct buttonStruct in activeButtons)
            {
                buttonStruct.CheckButton();
            }
        }
    }

    internal void SetUi(Building building)
    {
        Debug.Log("[Button Manager] Setting UI");
        ClearButtons();
        foreach (UnitSO unit in building.UnitOSList)
        {
            AddButton(unit, () => building.StartUnitProduction(unit));
        }
        UnitSelectionMenu.SetActive(true);
    }

    private void ClearButtons()
    {
        Debug.Log("[Button Manager] Clear Buttons");
        if (activeButtons.Count > 0)
        {
            foreach (ButtonStruct buttonStruct in activeButtons)
            {
                buttonStruct.DestroyButton();
            }
            activeButtons.Clear();
        }
    }

    private void AddButton(UnitSO unit, UnityAction onClick)
    {
        Debug.Log($"[Button Manager] Adding {unit.UnitName} button");
        ButtonStruct thing = new ButtonStruct(unit, UnitSelectionButtonPrefab, UnitSelectionMenu.transform,
                                              ColorActive, ColorInactive, onClick);
        activeButtons.Add(thing);
    }
}