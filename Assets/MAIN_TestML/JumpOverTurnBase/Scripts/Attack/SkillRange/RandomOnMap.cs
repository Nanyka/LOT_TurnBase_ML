using System.Collections.Generic;
using JumpeeIsland;
using UnityEngine;

public class RandomOnMap : SkillRange
{
    public Vector3 InvokeAt(Vector3 currentPos, Vector3 direction)
    {
        return currentPos;
    }

    public IEnumerable<Vector3> CalculateSkillRange(Vector3 currentPos, Vector3 direction, int step)
    {
        var returnPos = new List<Vector3>();
        returnPos.Add(GameFlowManager.Instance.GetEnvManager().GetRandomAvailableTile());
        return returnPos;
    }
}