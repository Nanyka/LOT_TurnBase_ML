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
        private List<MovableTile> _tileAreas = new();
        private List<Vector3> _potentialPos = new(4);
        private readonly AStar _aStarGrid = new();

        #region INIT SET UP

        private void Start()
        {
            GameFlowManager.Instance.OnStartGame.AddListener(InitiateAStar);
        }

        public void UpdateTileArea(MovableTile tilePos)
        {
            _tileAreas.Add(tilePos);
        }

        private void InitiateAStar(long arg0)
        {
            _aStarGrid.InitializeGrid(_tileAreas);
        }

        public List<Node> GetAStarPath(Vector3 starPos, Vector3 endPos)
        {
            _aStarGrid.UpdateObstacle();
            return _aStarGrid.FindPath(starPos, endPos);
        }

        // Get tile that allow player take jump
        public Vector3 GetPotentialTile()
        {
            GeneralAlgorithm.Shuffle(_tileAreas);

            foreach (var movableTile in _tileAreas)
            {
                var tile = movableTile.GetPosition();
                
                if (CheckFreeToMove(tile))
                    continue;

                if ((CheckFreeToMove(tile + Vector3.right) && CheckFreeToMove(tile + Vector3.left))
                    || (CheckFreeToMove(tile + Vector3.forward) && CheckFreeToMove(tile + Vector3.back)))
                {
                    _potentialPos.Clear();
                    if (CheckFreeToMove(tile + Vector3.left + Vector3.forward))
                        _potentialPos.Add(GetTileByGeoCoordinates(tile + Vector3.left + Vector3.forward).GetPosition());
                    if (CheckFreeToMove(tile + Vector3.left + Vector3.back))
                        _potentialPos.Add(GetTileByGeoCoordinates(tile + Vector3.left + Vector3.back).GetPosition());
                    if (CheckFreeToMove(tile + Vector3.right + Vector3.forward))
                        _potentialPos.Add(GetTileByGeoCoordinates(tile + Vector3.right + Vector3.forward).GetPosition());
                    if (CheckFreeToMove(tile + Vector3.right + Vector3.back))
                        _potentialPos.Add(GetTileByGeoCoordinates(tile + Vector3.right + Vector3.back).GetPosition());

                    if (_potentialPos.Count == 0)
                        continue;
                    return _potentialPos[Random.Range(0, _potentialPos.Count)];
                }
            }

            return Vector3.negativeInfinity;
        }
        
        public Vector3 GetAvailableTile()
        {
            GeneralAlgorithm.Shuffle(_tileAreas);

            foreach (var movableTile in _tileAreas)
            {
                var tile = movableTile.GetPosition();
                
                if (CheckFreeToMove(tile))
                    return tile;
            }

            return Vector3.negativeInfinity;
        }

        #endregion

        #region CHECK DOMAIN

        public bool CheckFreeToMove(Vector3 plot)
        {
            return CheckTileExist(plot) && !CheckObstacleAreas(plot);
        }

        public bool CheckTileExist(Vector3 plot)
        {
            plot = new (Mathf.RoundToInt(plot.x), Mathf.RoundToInt(plot.y), Mathf.RoundToInt(plot.z));
            return _tileAreas.Count(t => CheckGeoCoordinates(plot,t.GetPosition())) > 0;
        }

        private bool CheckObstacleAreas(Vector3 plot)
        {
            plot = new (Mathf.RoundToInt(plot.x), Mathf.RoundToInt(plot.y), Mathf.RoundToInt(plot.z));
            return _domainOwners.Any(owners =>
                owners.Value.Any(area => CheckGeoCoordinates(plot,area.transform.position)));
        }
        
        private bool CheckGeoCoordinates(Vector3 pos1 ,Vector3 pos2)
        {
            return Math.Abs(pos1.x - pos2.x) < 0.1f && Math.Abs(pos1.z - pos2.z) < 0.1f;
        }

        public bool CheckTeam(Vector3 position, FactionType faction)
        {
            var returnValue = false;
            position = new (Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));
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

        public bool CheckAlly(Vector3 targetPos, FactionType faction)
        {
            return CheckTeam(targetPos, faction);
        }

        public bool CheckTilesHeight(Vector3 tile1, Vector3 tile2)
        {
            return Math.Abs(GetTileByGeoCoordinates(tile1).GetPosition().y - GetTileByGeoCoordinates(tile2).GetPosition().y) < 0.1f;
        }
        
        public bool CheckHigherTile(Vector3 curPos, Vector3 checkPos)
        {
            return GetTileByGeoCoordinates(curPos).GetPosition().y < GetTileByGeoCoordinates(checkPos).GetPosition().y;
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

        public MovableTile GetTileByGeoCoordinates(Vector3 coordinates)
        {
            return _tileAreas.Find(t => t.CheckGeoCoordinates(coordinates));
        }

        #endregion
    }
}