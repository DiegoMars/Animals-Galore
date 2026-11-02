using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitOS", menuName = "Scriptable Objects/Unit")]
public class UnitOS : ScriptableObject
{
    [Header("General")]   
    public string UnitName;
    public int MaxHealth;
    public MovementTypes MovementType;

    public enum MovementTypes
    {
        GROUND,
        FLYING
    };
}