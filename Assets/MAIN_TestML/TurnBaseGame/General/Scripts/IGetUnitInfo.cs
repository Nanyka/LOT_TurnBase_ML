using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGetUnitInfo
{
    public (string name, int health, int damage, int power) GetUnitInfo();
    public (Vector3 midPos, Vector3 direction, int jumpStep, int faction) GetCurrentState();
    public EnvironmentController GetEnvironment();
}
