using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LOT_Turnbase
{
    public class DomainManager : MonoBehaviour
    {
        [SerializeField] protected GameObject _obstacle;
        // [SerializeField] protected Collider _platformColider;
        [SerializeField] protected int _numberOfObstacles;
        [SerializeField] protected bool _isDecidePosition;
        [SerializeField] protected Vector3 _designatedPostion;
        [SerializeField] private int _maxX;
        [SerializeField] private int _maxZ;

        private Dictionary<FactionType ,List<GameObject>> _domainOwners = new();
        private List<Vector3> _obstacleAreas = new();
        private List<Vector3> _tileAreas = new();

        #region INIT SET UP

        public void UpdateDomainOwner(GameObject domainOwner, FactionType factionType)
        {
            if (_domainOwners.ContainsKey(factionType) == false)
                _domainOwners.Add(factionType, new List<GameObject>());
            
            _domainOwners[factionType].Add(domainOwner);
        }

        public void SpawnObstacle()
        {
            UpdateTileArea();

            if (_isDecidePosition)
            {
                // var spawnPos = _designatedPostion + _platformColider.transform.position;
                // spawnPos = new Vector3(spawnPos.x, 0f, spawnPos.z);
                var obstacle = Instantiate(_obstacle, _designatedPostion, Quaternion.identity, transform);
                if (!obstacle.TryGetComponent(out Obstacle returnObstacle)) return;
                var occupyRange = returnObstacle.GetOccupyRange();
                foreach (var occupy in occupyRange)
                    _obstacleAreas.Add(occupy);
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

        private void UpdateTileArea()
        {
            foreach (var x in Enumerable.Range(0,_maxX))
                foreach (var z in Enumerable.Range(0,_maxZ))
                    _tileAreas.Add(new Vector3(x,0,z));
        }

        private Vector3 GetAvailablePlot()
        {
            var xPos = Mathf.RoundToInt(Random.Range(-_maxX, _maxX));
            var zPos = Mathf.RoundToInt(Random.Range(-_maxZ, _maxZ));
            var newPos = new Vector3(xPos, 0f, zPos);
            newPos = new Vector3(newPos.x, 0f, newPos.z);
            return CheckFreeToMove(newPos, true) ? newPos : GetAvailablePlot();
        }

        #endregion

        #region CHECK DOMAIN

        private bool CheckFreeToMove(Vector3 plot, bool isSpawningPhase)
        {
            if (isSpawningPhase && plot == Vector3.zero)
                return true;

            return CheckFreeToMove(plot);
        }

        public bool CheckFreeToMove(Vector3 plot)
        {
            return CheckTileExist(plot) && !CheckObstacleAreas(plot);
        }

        private bool CheckTileExist(Vector3 plot)
        {
            return _tileAreas.Contains(plot);
        }

        private bool CheckObstacleAreas(Vector3 checkPos)
        {
            return _obstacleAreas.Any(area => Vector3.Distance(checkPos, area) < 0.1f);
        }
        
        public bool CheckTeam(Vector3 position, int faction)
        {
            var returnValue = false;
            var listByFaction = faction == 0 ? _domainOwners[FactionType.Player] : _domainOwners[FactionType.Enemy];
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

        public bool CheckEnemy(Vector3 targetPos, int faction)
        {
            if (faction == 0)
                return CheckTeam(targetPos, 1);
            return CheckTeam(targetPos, 0);
        }

        #endregion

        #region UPDATE DOMAIN

        public void DestroyAtPosition(Vector3 position)
        {
            var findPos = _domainOwners[FactionType.Enemy].Find(x => Vector3.Distance(x.transform.position, position) < Mathf.Epsilon);
            if (findPos == null) return;
            Destroy(findPos);
            _domainOwners[FactionType.Enemy].Remove(findPos);
        }

        public int CountObstacle()
        {
            return _domainOwners[FactionType.Enemy].Count;
        }
        
        public GameObject GetEnemyByPosition(Vector3 position, int fromFaction)
        {
            return fromFaction == 0
                ? _domainOwners[FactionType.Enemy].Find(x => Vector3.Distance(x.transform.position, position) < 0.1f)
                : _domainOwners[FactionType.Player].Find(x => Vector3.Distance(x.transform.position, position) < 0.1f);
        }

        public void RemoveObject(GameObject targetObject, int faction)
        {
            if (faction == 0)
            {
                _domainOwners[FactionType.Player].Remove(targetObject);
                _domainOwners[FactionType.Player] = _domainOwners[FactionType.Player].Where(x => x != null).ToList();
            }
            else
            {
                _domainOwners[FactionType.Enemy].Remove(targetObject);
                _domainOwners[FactionType.Enemy] = _domainOwners[FactionType.Enemy].Where(x => x != null).ToList();
            }
        }

        #endregion

        public int CheckWinCondition()
        {
            if (_domainOwners[FactionType.Enemy].Count == 0)
                return 0;
            if (_domainOwners[FactionType.Player].Count == 0)
                return 1;
            return -1;
        }
    }
}