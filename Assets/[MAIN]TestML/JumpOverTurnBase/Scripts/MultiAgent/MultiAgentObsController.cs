using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiAgentObsController : ObstacleManager
{
    private List<Vector3> _obstacleAreas = new();

    public override void SpawnObstacle()
    {
        SetUpPlatform();

        if (_isDecidePosition)
        {
            var spawnPos = _designatedPostion + _platformColider.transform.position;
            spawnPos = new Vector3(spawnPos.x, 0f, spawnPos.z);
            var obstacle = Instantiate(_obstacle, spawnPos, Quaternion.identity, transform);
            if (obstacle.TryGetComponent(out Obstacle returnObstacle))
            {
                var occupyRange = returnObstacle.GetOccupyRange();
                foreach (var occupy in occupyRange)
                    _obstacleAreas.Add(occupy);
            }
        }
        else
        {
            for (int i = 0; i < _numberOfObstacles; i++)
            {
                var spawnPos = GetAvailablePlot();
                var obstacle = Instantiate(_obstacle, spawnPos, Quaternion.identity, transform);
                if (obstacle.TryGetComponent(out Obstacle returnObstacle))
                {
                    var occupyRange = returnObstacle.GetOccupyRange();
                    foreach (var occupy in occupyRange)
                        _obstacleAreas.Add(occupy);
                }
            }
        }
    }

    protected override bool CheckObstaclePlot(Vector3 plot, bool isSpawningPhase)
    {
        if (isSpawningPhase && plot == Vector3.zero)
            return true;

        return CheckObstaclePlot(plot);
    }

    public override bool CheckObstaclePlot(Vector3 plot)
    {
        return CheckTeam(plot, 0) || CheckTeam(plot, 1) || CheckObstacleAreas(plot);
    }

    public override bool CheckTeam(Vector3 position, int faction)
    {
<<<<<<< HEAD
        if (faction == 0)
            return _listTeam0.Find(x => Vector3.Distance(x.transform.position, position) < 0.1f);
        return _listTeam1.Find(x => Vector3.Distance(x.transform.position, position) < 0.1f);
=======
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

    private bool CheckObstacleAreas(Vector3 checkPos)
    {
        var returnValue = false;
        foreach (var area in _obstacleAreas)
        {
            if (Vector3.Distance(checkPos,area) < 0.1f)
            {
                returnValue = true;
                break;
            }
        }

        return returnValue;
>>>>>>> testSkillManager
    }
}