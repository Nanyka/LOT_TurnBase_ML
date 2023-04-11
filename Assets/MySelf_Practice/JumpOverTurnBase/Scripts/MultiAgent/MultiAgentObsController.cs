using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiAgentObsController : ObstacleManager
{
    protected override bool CheckObstaclePlot(Vector3 plot, bool isSpawningPhase)
    {
        if (isSpawningPhase && plot == Vector3.zero)
            return true;

        return CheckObstaclePlot(plot);
    }

    public override bool CheckObstaclePlot(Vector3 plot)
    {
        return CheckTeam(plot, 0) || CheckTeam(plot, 1);
    }

    public override bool CheckTeam(Vector3 position, int faction)
    {
        var returnValue = false;
        var listByFaction = faction == 0 ? _listTeam0 : _listTeam1;
        foreach (var item in listByFaction)
        {
            if (item == null)
                continue;

            if (Vector3.Distance(item.transform.position, position) < 0.1f)
            {
                returnValue = true;
                break;
            }
        }

        return returnValue;
    }
}