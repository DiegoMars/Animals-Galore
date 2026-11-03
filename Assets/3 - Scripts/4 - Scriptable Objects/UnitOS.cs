using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitOS", menuName = "Scriptable Objects/Unit")]
public class UnitOS : ScriptableObject
{
    public enum MovementTypesEnum
    {
        GROUND,
        FLYING
    };
    public enum DamageTypeEnum
    {
        Melee,
        Range
    };

    public GameObject prefab;
    public string UnitName;
    public FactionManager.Factions Faction;
    public bool Harvestor;
    public int MaxHealth;
    public MovementTypesEnum MovementType;
    public int Damage;
    public DamageTypeEnum DamageType;
    public int MovementSpeed;
    public int cost;
    public ResourceManager.ResourceTypes costType;
}