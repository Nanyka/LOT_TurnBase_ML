using UnityEngine;

public interface ICheckEnemyPosition
{
    public bool CheckEnemy(Vector3 checkPos, int faction);
    public GameObject GetEnemyByPosition(Vector3 checkPos, int faction);
}