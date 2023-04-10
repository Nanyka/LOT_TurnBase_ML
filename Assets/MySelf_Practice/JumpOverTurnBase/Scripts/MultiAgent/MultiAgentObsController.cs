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
        if (faction == 0)
            return _listTeam0.Find(x => Vector3.Distance(x.transform.position, position) < 0.1f);
        return _listTeam1.Find(x => Vector3.Distance(x.transform.position, position) < 0.1f);
    }
}