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

    public string UnitName;
    public int MaxHealth;
    public MovementTypesEnum MovementType;
    public int Damage;
    public DamageTypeEnum DamageType;
    public int MovementSpeed;
    public FactionManager.Factions Faction;
    public bool Harvestor;
    public GameObject prefab;
}