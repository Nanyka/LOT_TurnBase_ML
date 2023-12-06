using System.Collections.Generic;
using System.Linq;
using JumpeeIsland;
using UnityEngine;

public class AccurateAttackByHp : SkillRange
{
    public Vector3 InvokeAt(Vector3 currentPos, Vector3 direction)
    {
        return currentPos;
    }

    public IEnumerable<Vector3> CalculateSkillRange(Vector3 currentPos, Vector3 direction, int step)
    {
        var currentEntity = GameFlowManager.Instance.GetEnvManager().CheckFaction(currentPos);
        var enemies = SavingSystemManager.Instance.GetEnvironmentData().PlayerData;
        if (currentEntity == FactionType.Player)
            enemies = SavingSystemManager.Instance.GetEnvironmentData().EnemyData;

        enemies.Sort((a, b) => b.CurrentHp.CompareTo(a.CurrentHp));
        var returnPos = new List<Vector3>();
        step = enemies.Count > step ? step : enemies.Count;
        for (int i = 0; i < step; i++)
            returnPos.Add(enemies[i].Position);
        return returnPos;
    }
}