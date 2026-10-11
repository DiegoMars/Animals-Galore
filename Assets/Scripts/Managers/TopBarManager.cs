using System;
using TMPro;
using UnityEngine;

public class TopBarManager : MonoBehaviour
{
    public static TopBarManager Instance {get; set;}

    [Header("General")]
    public TextMeshProUGUI WoodQuantity;
    public TextMeshProUGUI FoodQuantity;
    public TextMeshProUGUI StoneQuantity;
    public TextMeshProUGUI TechPointsQuantity;
    public TextMeshProUGUI Population;
    public TextMeshProUGUI Age;
    
    private int agesPos = 0;
    private string[] ages =
    {
        "1 - Birth Age",
        "2 - Growing Age",
        "3 - Amber Age"
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

    void Start()
    {
        WoodQuantity.text = "0";
        FoodQuantity.text = "0";
        StoneQuantity.text = "0";
        TechPointsQuantity.text = "0";
        Population.text = "0";
        Age.text = ages[agesPos];
    }

    public void SetWood(String value)
    {
        WoodQuantity.SetText(value);
    }
    public void SetFood(String value)
    {
        FoodQuantity.SetText(value);
    }
    public void SetStone(String value)
    {
        StoneQuantity.SetText(value);
    }
    public void SetTech(String value)
    {
        TechPointsQuantity.SetText(value);
    }
    public void UpdatePopulation()
    {
        int population = UnitSelectionManager.Instance.allUnitsList.Count;
        Population.SetText(population.ToString());
    }
    public void UpInAge()
    {
        if (agesPos < ages.Length - 1)
        {
            agesPos += 1;
            Age.SetText(ages[agesPos]);
        }
        else
        {
            Debug.Log($"Age {agesPos} > {ages.Length}");
        }
    }
}
