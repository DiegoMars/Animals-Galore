using NUnit.Framework.Internal;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingSO", menuName = "Scriptable Objects/Building")]
public class BuildingSO : ScriptableObject
{
    public string BuildingName;
    public GameObject Prefab;
    public UnitSO[] UnitSOList;
    public int MaxHealth;
    public int Cost;
    public ResourceManager.ResourceTypes CostType;
}