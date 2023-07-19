using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JumpeeIsland
{
    public class DomainManager : MonoBehaviour
    {
        private Dictionary<FactionType, List<GameObject>> _domainOwners = new();
        private List<Vector3> _tileAreas = new();
        private List<Vector3> _potentialPos = new List<Vector3>(4);

        #region INIT SET UP

        public void UpdateTileArea(Vector3 tilePos)
        {
            _tileAreas.Add(tilePos);
        }

        // Get tile that allow player take jump
        public Vector3 GetPotentialTile()
        {
            GeneralAlgorithm.Shuffle(_tileAreas);

            foreach (var tile in _tileAreas)
            {
                if (CheckFreeToMove(tile))
                    continue;

                if ((CheckFreeToMove(tile + Vector3.right) && CheckFreeToMove(tile + Vector3.left))
                    || (CheckFreeToMove(tile + Vector3.forward) && CheckFreeToMove(tile + Vector3.back)))
                {
                    _potentialPos.Clear();
                    if (CheckFreeToMove(tile + Vector3.left + Vector3.forward))
                        _potentialPos.Add(tile + Vector3.left + Vector3.forward);
                    if (CheckFreeToMove(tile + Vector3.left + Vector3.back))
                        _potentialPos.Add(tile + Vector3.left + Vector3.back);
                    if (CheckFreeToMove(tile + Vector3.right + Vector3.forward))
                        _potentialPos.Add(tile + Vector3.right + Vector3.forward);
                    if (CheckFreeToMove(tile + Vector3.right + Vector3.back))
                        _potentialPos.Add(tile + Vector3.right + Vector3.back);

                    if (_potentialPos.Count == 0)
                        continue;
                    return _potentialPos[Random.Range(0, _potentialPos.Count)];
                }
            }

            return Vector3.negativeInfinity;
        }

        #endregion

        #region CHECK DOMAIN

        public bool CheckFreeToMove(Vector3 plot)
        {
            return CheckTileExist(plot) && !CheckObstacleAreas(plot);
        }

        private bool CheckTileExist(Vector3 plot)
        {
            plot = new (Mathf.RoundToInt(plot.x), Mathf.RoundToInt(plot.y), Mathf.RoundToInt(plot.z));
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
        
        public bool CheckOneFactionZeroTroop()
        {
            return _domainOwners[FactionType.Enemy].Count == 0 || _domainOwners[FactionType.Player].Count == 0;
        }

        #endregion

        #region UPDATE DOMAIN

        private List<GameObject> GetListObjByFaction(FactionType factionType)
        {
            if (_domainOwners.ContainsKey(factionType) == false)
                _domainOwners.Add(factionType, new());
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
            if (_domainOwners.ContainsKey(fromFaction) == false)
                _domainOwners.Add(fromFaction, new List<GameObject>());

            return _domainOwners[fromFaction].Find(x => Vector3.Distance(x.transform.position, position) < 0.1f);
        }

        public void RemoveObject(GameObject targetObject, FactionType faction)
        {
            _domainOwners[faction].Remove(targetObject);
            _domainOwners[faction] = _domainOwners[faction].Where(x => x != null).ToList();
        }

        #endregion

        #region GET

        public int CountFaction(FactionType factionType)
        {
            return _domainOwners.TryGetValue(factionType, out var owner)?owner.Count:0;
        }

        #endregion
    }
}