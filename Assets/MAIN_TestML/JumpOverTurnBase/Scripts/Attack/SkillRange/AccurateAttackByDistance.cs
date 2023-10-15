using System.Collections.Generic;
using System.Linq;
using JumpeeIsland;
using UnityEngine;

public class AccurateAttackByDistance : SkillRange
{
    private int _range;

    public AccurateAttackByDistance(int range)
    {
        _range = range;
    }

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

        var returnPos = new List<Vector3>();
        foreach (var enemy in enemies)
        {
            if (Vector3.Distance(enemy.Position, currentPos) <= _range * 1f)
                returnPos.Add(enemy.Position);
        }
        
        returnPos.Sort(new DistanceComparer(currentPos));
        step = returnPos.Count > step ? step : returnPos.Count;
        int countToRemove = returnPos.Count - step; // Number of elements to remove
        returnPos.RemoveRange(step, countToRemove);
        return returnPos;
    }
}