using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.ExceptionServices;
using NUnit.Framework.Constraints;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("General")]
    public static ButtonManager Instance {get; set;}
    public GameObject UnitSelectionMenu;
    public GameObject UnitSelectionButtonPrefab;
    public Color32 ColorActive;
    public Color32 ColorInactive;
    public List<ButtonStruct> activeButtons = new();

    private struct ButtonStruct
    {
        private int Cost;
        private ResourceManager.ResourceTypes CostType;
        private GameObject UnitButton;
        private Button ActualButton;
        public Color32 ColorActive;
        public Color32 ColorInactive;
        public ButtonStruct(String Name, int cost,
                            ResourceManager.ResourceTypes costType,
                            GameObject UnitButtonPrefab, Transform Menu,
                            Color32 colorActive, Color32 colorInactive)
        {
            Cost = cost;
            CostType = costType;
            UnitButton = Instantiate(UnitButtonPrefab, Menu);
            ActualButton = UnitButton.GetComponentInChildren<Button>();
            ColorActive = colorActive;
            ColorInactive = colorInactive;
            
            bool nameFlag = true;
            bool unitCostFlag = true;
            foreach (TextMeshProUGUI textObject in
                     UnitButton.GetComponentsInChildren<TextMeshProUGUI>())
            {
                if (textObject.gameObject.name == "UnitName")
                {
                    textObject.text = Name;
                    nameFlag = false;
                }
                if (textObject.gameObject.name == "UnitCost")
                {
                    textObject.text = $"{cost.ToString()} {costType.ToString()}";
                    unitCostFlag = false;
                }
            }
            if (nameFlag || unitCostFlag)
            {
                Debug.Log("[Button Manager : Button Struct] 1 or more meshes not found");
            }
        }
        
        public void CheckButton()
        {

            if (ResourceManager.Instance.ResourceQuantity(CostType) >= Cost)
            {
                ActualButton.enabled = true; 
                ActualButton.image.color = ColorActive;
            }
            else
            {
                ActualButton.enabled = false; 
                ActualButton.image.color = ColorInactive;
            }
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
        // if (ResourceManager.Instance.ResourceQuantity(UnitCostType) >= UnitCostValue)
        // {
        //     EnableButton();
        // }
        // else
        // {
        //     DisableButton();
        // }
    }

    internal void SetUi(UnitOS[] UnitOSList)
    {
        foreach (UnitOS unit in UnitOSList)
        {

        }
    }

    private bool SetButtonText(GameObject UnitButton, String UnitName, int UnitCost,
                               ResourceManager.ResourceTypes UnitCostType)
    {
        throw new NotImplementedException();
    }
}