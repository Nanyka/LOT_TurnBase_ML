using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleManager : MonoBehaviour
{
    [SerializeField] private GameObject _obstacle;
    [SerializeField] private Collider _platformColider;
    [SerializeField] private int _numberOfObstacles;
    [SerializeField] private bool _isDecidePosition;
    [SerializeField] private Vector3 _designatedPostion;
    [SerializeField] protected List<GameObject> _listTeam0 = new();
    [SerializeField] protected List<GameObject> _listTeam1 = new();

    private int _maxX;
    private int _maxZ;

    public void SpawnObstacle()
    {
        SetUpPlatform();

        if (_isDecidePosition)
        {
            var spawnPos = _designatedPostion + _platformColider.transform.position;
            spawnPos = new Vector3(spawnPos.x, 0f, spawnPos.z);
            var obstacle = Instantiate(_obstacle, spawnPos, Quaternion.identity, transform);
            _listTeam1.Add(obstacle);
        }
        else
        {
            for (int i = 0; i < _numberOfObstacles; i++)
            {
                var spawnPos = GetAvailablePlot();
                var obstacle = Instantiate(_obstacle, spawnPos, Quaternion.identity, transform);
                _listTeam1.Add(obstacle);
            }
        }
    }

    private void SetUpPlatform()
    {
        var platformSize = _platformColider.bounds.size;
        _maxX = Mathf.RoundToInt((platformSize.x - 1) / 2) - 1;
        _maxZ = Mathf.RoundToInt((platformSize.z - 1) / 2) - 1;

        foreach (var obstacle in _listTeam1)
            Destroy(obstacle);

        _listTeam1.Clear();
    }

    private Vector3 GetAvailablePlot()
    {
        var xPos = Mathf.RoundToInt(Random.Range(-_maxX, _maxX));
        var zPos = Mathf.RoundToInt(Random.Range(-_maxZ, _maxZ));
        var newPos = new Vector3(xPos, 0f, zPos) + _platformColider.transform.position;
        newPos = new Vector3(newPos.x, 0f, newPos.z);
        if (CheckObstaclePlot(newPos, true))
            return GetAvailablePlot();
        return newPos;
    }

    protected virtual bool CheckObstaclePlot(Vector3 plot, bool isSpawningPhase)
    {
        if (isSpawningPhase && plot == Vector3.zero)
            return true;

        return CheckObstaclePlot(plot);
    }

    public virtual bool CheckObstaclePlot(Vector3 plot)
    {
        return _listTeam1.Find(x => Vector3.Distance(x.transform.position, plot) < Mathf.Epsilon);
    }

    public void DestroyAtPosition(Vector3 position)
    {
        var findPos = _listTeam1.Find(x => Vector3.Distance(x.transform.position, position) < Mathf.Epsilon);
        if (findPos == null) return;
        Destroy(findPos);
        _listTeam1.Remove(findPos);
    }

    public int CountObstacle()
    {
        return _listTeam1.Count;
    }

    public virtual bool CheckTeam(Vector3 position, int faction)
    {
        return true;
    }

    public bool CheckEnemy(Vector3 targetPos, int faction)
    {
        if (faction == 0)
            return CheckTeam(targetPos, 1);
        return CheckTeam(targetPos, 0);
    }

    public GameObject GetEnemyByPosition(Vector3 position, int fromFaction)
    {
        if (fromFaction == 0)
            return _listTeam1.Find(x => Vector3.Distance(x.transform.position, position) < 0.1f);
        return _listTeam0.Find(x => Vector3.Distance(x.transform.position, position) < 0.1f);
    }
}