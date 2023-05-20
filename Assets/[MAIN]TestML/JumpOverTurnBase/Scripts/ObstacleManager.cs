using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleManager : MonoBehaviour
{
    [SerializeField] protected GameObject _obstacle;
    [SerializeField] protected Transform _obstacleContainer;
    [SerializeField] protected Collider _platformColider;
    [SerializeField] protected int _numberOfObstacles;
    [SerializeField] protected bool _isDecidePosition;
    [SerializeField] protected Vector3 _designatedPostion;
    protected Dictionary<FactionType, List<GameObject>> _teams;

    private int _maxX;
    private int _maxZ;

    private void Start()
    {
        ResetFactionDictionary();
    }

    private void ResetFactionDictionary()
    {
        _teams = new Dictionary<FactionType, List<GameObject>>
        {
            {FactionType.player, new List<GameObject>()}, {FactionType.enemy, new List<GameObject>()}
        };
    }

    public void AddFaction(FactionType factionType, GameObject targetObject)
    {
        if (_teams.ContainsKey(factionType) == false)
            _teams.Add(factionType,new List<GameObject>());
        _teams[factionType].Add(targetObject);
    }

    public virtual void SpawnObstacle()
    {
        SetUpPlatform();

        foreach (var obstacle in _teams[FactionType.enemy])
            Destroy(obstacle);
        _teams[FactionType.enemy].Clear();

        if (_isDecidePosition)
        {
            var spawnPos = _designatedPostion + _platformColider.transform.position;
            spawnPos = new Vector3(spawnPos.x, 0f, spawnPos.z);
            var obstacle = Instantiate(_obstacle, spawnPos, Quaternion.identity, _obstacleContainer);
            _teams[FactionType.enemy].Add(obstacle);
        }
        else
        {
            for (int i = 0; i < _numberOfObstacles; i++)
            {
                var spawnPos = GetAvailablePlot();
                var obstacle = Instantiate(_obstacle, spawnPos, Quaternion.identity, _obstacleContainer);
                _teams[FactionType.enemy].Add(obstacle);
            }
        }
    }

    protected void SetUpPlatform()
    {
        var platformSize = _platformColider.bounds.size;
        _maxX = Mathf.RoundToInt((platformSize.x - 1) / 2) - 1;
        _maxZ = Mathf.RoundToInt((platformSize.z - 1) / 2) - 1;
    }

    protected Vector3 GetAvailablePlot()
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
        return _teams[FactionType.enemy].Find(x => Vector3.Distance(x.transform.position, plot) < Mathf.Epsilon);
    }

    public void DestroyAtPosition(Vector3 position)
    {
        var findPos = _teams[FactionType.enemy].Find(x => Vector3.Distance(x.transform.position, position) < Mathf.Epsilon);
        if (findPos == null) return;
        Destroy(findPos);
        _teams[FactionType.enemy].Remove(findPos);
    }

    public int CountObstacle()
    {
        return _teams[FactionType.enemy].Count;
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
            return _teams[FactionType.enemy].Find(x => Vector3.Distance(x.transform.position, position) < 0.1f);
        return _teams[FactionType.player].Find(x => Vector3.Distance(x.transform.position, position) < 0.1f);
    }

    public void RemoveObject(GameObject targetObject, int faction)
    {
        if (faction == 0)
        {
            _teams[FactionType.player].Remove(targetObject);
            _teams[FactionType.player] = _teams[FactionType.player].Where(x => x != null).ToList();
        }
        else
        {
            _teams[FactionType.enemy].Remove(targetObject);
            _teams[FactionType.enemy] = _teams[FactionType.enemy].Where(x => x != null).ToList();
        }
    }

    public int CheckWinCondition()
    {
        if (_teams[FactionType.enemy].Count == 0)
            return 0;
        if (_teams[FactionType.player].Count == 0)
            return 1;
        return -1;
    }

    public int GetAmountOfFaction()
    {
        return _teams.Count;
    }
}