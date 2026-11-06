using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitOS", menuName = "Scriptable Objects/Unit")]
public class UnitSO : ScriptableObject
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

    [Header("General")]
    public GameObject prefab;
    public string UnitName;
    public FactionManager.Factions Faction;
    public bool Harvestor; // A troop otherwise.
    public MovementTypesEnum MovementType;
    public DamageTypeEnum DamageType;
    public ResourceManager.ResourceTypes costType;
    [Header("Stats")]
    public int MaxHealth;
    public int Damage;
    public float AttackRange;
    public float AttackCooldown;
    public float MovementSpeed;
    public int cost;
    public float CollectionTime;
    public float DetectionRange;
}