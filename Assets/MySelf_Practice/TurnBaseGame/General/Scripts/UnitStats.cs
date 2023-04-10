using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStats", menuName = "TurnBase/UnitStats", order = 2)]
public class UnitStats : ScriptableObject
{
    public int HealthPoint;
    public int Strengh;
    public int Agility;
}