using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JumpeeIsland
{
    public class DomainManager : MonoBehaviour
    {
        // [SerializeField] protected GameObject _obstacle;
        // [SerializeField] protected Transform _obstacleContainer;
        // [SerializeField] protected int _numberOfObstacles;
        // [SerializeField] protected bool _isDecidePosition;
        // [SerializeField] protected Vector3 _designatedPostion;
        // [SerializeField] private int _maxX;
        // [SerializeField] private int _maxZ;
        //
        // [SerializeField] protected GameObject _tileForTesting; // REMOVE when done environment testing
        // [SerializeField] protected Transform _tileContainer; // REMOVE when done environment testing

        private Dictionary<FactionType, List<GameObject>> _domainOwners = new();
        // private List<Vector3> _obstacleAreas = new();
        private List<Vector3> _tileAreas = new();

        #region INIT SET UP

        public void UpdateTileArea(Vector3 tilePos)
        {
            _tileAreas.Add(tilePos);
        }

        // private Vector3 GetAvailablePlot()
        // {
        //     var xPos = Mathf.RoundToInt(Random.Range(-_maxX, _maxX));
        //     var zPos = Mathf.RoundToInt(Random.Range(-_maxZ, _maxZ));
        //     var newPos = new Vector3(xPos, 0f, zPos);
        //     newPos = new Vector3(newPos.x, 0f, newPos.z);
        //     return CheckFreeToMove(newPos, true) ? newPos : GetAvailablePlot();
        // }

        #endregion

        #region CHECK DOMAIN

        // private bool CheckFreeToMove(Vector3 plot, bool isSpawningPhase)
        // {
        //     if (isSpawningPhase && plot == Vector3.zero)
        //         return true;
        //
        //     return CheckFreeToMove(plot);
        // }

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
            return _domainOwners.Any(owners =>
                owners.Value.Any(area => Vector3.Distance(checkPos, area.transform.position) < 0.1f));
        }

        public bool CheckTeam(Vector3 position, FactionType faction)
        {
            var returnValue = false;
            var listByFaction = GetListObjByFaction(faction);
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

        public bool CheckEnemy(Vector3 targetPos, FactionType faction)
        {
            if (faction == FactionType.Player)
                return CheckTeam(targetPos, FactionType.Enemy);
            return CheckTeam(targetPos, FactionType.Player);
        }

        #endregion

        #region UPDATE DOMAIN

        private List<GameObject> GetListObjByFaction(FactionType factionType)
        {
            if (_domainOwners.ContainsKey(factionType) == false)
                _domainOwners.Add(factionType,new());
            return _domainOwners[factionType];
        }

        public void UpdateDomainOwner(GameObject domainOwner, FactionType factionType)
        {
            GetListObjByFaction(factionType).Add(domainOwner);
        }

        public int CountObstacle()
        {
            return _domainOwners[FactionType.Enemy].Count;
        }

        public GameObject GetObjectByPosition(Vector3 position, FactionType fromFaction)
        {
            return _domainOwners[fromFaction].Find(x => Vector3.Distance(x.transform.position, position) < 0.1f);
        }

        public void RemoveObject(GameObject targetObject, FactionType faction)
        {
            _domainOwners[faction].Remove(targetObject);
            _domainOwners[faction] = _domainOwners[faction].Where(x => x != null).ToList();
        }

        #endregion

        public FactionType CheckWinCondition()
        {
            if (_domainOwners[FactionType.Enemy].Count == 0)
                return FactionType.Player;
            if (_domainOwners[FactionType.Player].Count == 0)
                return FactionType.Enemy;
            return FactionType.Neutral;
        }
    }
}