using System.Collections;
using System.Collections.Generic;
using JumpeeIsland;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStats", menuName = "TurnBase/UnitStats", order = 2)]
public class UnitStats : ScriptableObject
{
    public int HealthPoint;
    public int Strengh;
    public int Agility;
    [Tooltip("Amount of exp to level up this unit")]
    public int ExpToLevelUp;
    [Tooltip("Amount of exp that entity destroying this resource can collect")]
    public int ExpReward;
    public CommandName Command;
}