using System.Collections;
using System.Collections.Generic;
using JumpeeIsland;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "UnitStats", menuName = "JumpeeIsland/UnitStats", order = 2)]
public class UnitStats : ScriptableObject
{
    public int HealthPoint;
    public int Strengh;
    public int Armor;
    [Tooltip("Amount of exp to level up this unit")]
    public int CostToLevelUp;
    [Tooltip("Amount of exp that entity destroying this resource can collect")]
    public int ExpReward;
    public CommandName[] Commands;
    public CreatureType CreatureType;
}