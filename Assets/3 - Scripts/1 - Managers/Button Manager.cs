using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("General")]
    public static ButtonManager Instance {get; set;}
    public GameObject UnitSelectionMenu;
    public GameObject UnitSelectionButtonPrefab;
    public GameObject BuildingSelectionMenu;
    public GameObject BuildingSelectionButtonPrefab;
    public Color32 ColorActive;
    public Color32 ColorInactive;

    private List<UnitButtonStruct> ActiveUnitButtons = new();
    private bool BuildingButtonsAreSet = false;
    private List<BuildingButtonStruct> ActiveBuildingButtons = new();
    [SerializeField] private List<KeyCode> KeysToUse = new() {KeyCode.Q, KeyCode.W, KeyCode.A, KeyCode.S};
    private int KeyToUseNext = 0;
    private Dictionary<KeyCode, BuildingSO> buildingKeys = new Dictionary<KeyCode, BuildingSO>();

    private struct UnitButtonStruct
    {
        UnitSO unitSOb;
        private GameObject UnitButton;
        private Button ActualButton;
        public Color32 ColorActive;
        public Color32 ColorInactive;
        public UnitButtonStruct(UnitSO unitSO, GameObject UnitButtonPrefab, Transform Menu,
                            Color32 colorActive, Color32 colorInactive, UnityAction onClick)
        {
            unitSOb = unitSO;
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
                    textObject.text = unitSOb.UnitName;
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
        }
        
        public void CheckButton()
        {

            bool canAfford = ResourceManager.Instance.ResourceQuantity(FactionManager.Instance.PlayerFaction, unitSOb.costType) >= unitSOb.cost;
            ActualButton.enabled = canAfford; 
            ActualButton.image.color = canAfford ? ColorActive : ColorInactive;
        }
        
        public void DestroyButton()
        {
            Destroy(UnitButton);
        }
    }

    private struct BuildingButtonStruct
    {
        BuildingSO buildingSOb;
        private GameObject BuildingButton;
        private Button ActualButton;
        public Color32 ColorActive;
        public Color32 ColorInactive;
        public BuildingButtonStruct(BuildingSO buildingSO, GameObject BuildingButtonPrefab, Transform Menu,
                            Color32 colorActive, Color32 colorInactive, UnityAction onClick, KeyCode key)
        {
            buildingSOb = buildingSO;
            BuildingButton = Instantiate(BuildingButtonPrefab, Menu);
            ActualButton = BuildingButton.GetComponentInChildren<Button>();
            ColorActive = colorActive;
            ColorInactive = colorInactive;
            
            ActualButton.onClick.AddListener(onClick);
            
            bool nameFlag = true;
            bool costFlag = true;
            bool keyFlag = true;
            foreach (TextMeshProUGUI textObject in
                     BuildingButton.GetComponentsInChildren<TextMeshProUGUI>())
            {
                if (textObject.gameObject.name == "BuildingName")
                {
                    textObject.text = buildingSOb.BuildingName;
                    nameFlag = false;
                }
                if (textObject.gameObject.name == "BuildingCost")
                {
                    textObject.text = $"{buildingSOb.Cost.ToString()} {buildingSOb.CostType.ToString()}";
                    costFlag = false;
                }
                if (textObject.gameObject.name == "BuildingKey")
                {
                    textObject.text = $"{key}";
                    keyFlag = false;
                }
            }
            if (nameFlag || costFlag || keyFlag)
            {
                Debug.Log("[Button Manager : Button Struct] 1 or more meshes not found");
            }
        }
        
        public void CheckButton()
        {
            bool canAfford = ResourceManager.Instance.ResourceQuantity(FactionManager.Instance.PlayerFaction,
                                                                       buildingSOb.CostType) >= buildingSOb.Cost;
            ActualButton.enabled = canAfford; 
            ActualButton.image.color = canAfford ? ColorActive : ColorInactive;
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
        // This is such a janky fix LMAOOO. But if it works it works
        if (!BuildingButtonsAreSet)
        {
            SetBuildingButtonsUi(BuildManager.Instance.BuildingSOs);
            BuildingButtonsAreSet = true;
        }
        if (Input.anyKeyDown) 
        {
            foreach (KeyCode key in buildingKeys.Keys)
            {
                if (Input.GetKeyDown(key))
                {
                    BuildManager.Instance.selectBuildingPrefab(buildingKeys[key]);
                    break; 
                }
            }
        }
        if (ActiveUnitButtons.Count > 0)
        {
            foreach (UnitButtonStruct buttonStruct in ActiveUnitButtons)
            {
                buttonStruct.CheckButton();
            }
        }
        if (ActiveBuildingButtons.Count > 0)
        {
            foreach (BuildingButtonStruct buttonStruct in ActiveBuildingButtons)
            {
                buttonStruct.CheckButton();
            }
        }
    }

    internal void SetBuildingButtonsUi(BuildingSO[] buildings)
    {
        Debug.Log("[Button Manager] Setting Buildings UI");
        foreach (BuildingSO building in buildings)
        {
            AddButton(building, () => BuildManager.Instance.selectBuildingPrefab(building));
        }
    }
    internal void SetUnitButtonsUi(Building building)
    {
        Debug.Log("[Button Manager] Setting Unit Buttons UI");
        ClearUnitButtons();
        foreach (UnitSO unit in building.buidlingSO.UnitSOList)
        {
            AddButton(unit, () => building.StartUnitProduction(unit));
        }
        UnitSelectionMenu.SetActive(true);
    }

    private void ClearUnitButtons()
    {
        Debug.Log("[Button Manager] Clear Buttons");
        if (ActiveUnitButtons.Count > 0)
        {
            foreach (UnitButtonStruct buttonStruct in ActiveUnitButtons)
            {
                buttonStruct.DestroyButton();
            }
            ActiveUnitButtons.Clear();
        }
    }

    private void AddButton(UnitSO unit, UnityAction onClick)
    {
        Debug.Log($"[Button Manager] Adding {unit.UnitName} button");
        UnitButtonStruct thing = new UnitButtonStruct(unit, UnitSelectionButtonPrefab, UnitSelectionMenu.transform,
                                              ColorActive, ColorInactive, onClick);
        ActiveUnitButtons.Add(thing);
    }
    private void AddButton(BuildingSO building, UnityAction onClick)
    {
        Debug.Log($"[Button Manager] Adding {building.BuildingName} button");
        KeyCode key = KeysToUse[KeyToUseNext];
        KeyToUseNext += 1;
        buildingKeys.Add(key, building);
        BuildingButtonStruct thing = new BuildingButtonStruct(building, BuildingSelectionButtonPrefab, BuildingSelectionMenu.transform,
                                              ColorActive, ColorInactive, onClick, key);
        ActiveBuildingButtons.Add(thing);
    }
}