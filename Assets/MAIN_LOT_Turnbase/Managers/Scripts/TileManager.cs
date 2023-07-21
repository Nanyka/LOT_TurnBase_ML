using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class TileManager : MonoBehaviour
    {
        // [Serializable]
        // public class ExpandPoint
        // {
        //     public int tileAmount;
        //     public int score;
        //
        //     public bool CheckToExpand(int currentTileAmount, int currentScore)
        //     {
        //         return currentTileAmount < tileAmount && currentScore >= score;
        //     }
        // }

        // [SerializeField] private ExpandPoint[] _expandPoints;
        [SerializeField] private ObjectPool _tilePool;
        [FormerlySerializedAs("_totalTile")] [SerializeField] private int _mapIndex;
        [SerializeField] private MapContainer m_MapContainer;
        
        private List<Transform> m_Tiles;
        private List<TilePosition> _listTilePos = new();

        public void Init(int mapIndex)
        {
            // SavingSystemManager.Instance.OnCheckExpandMap.AddListener(CheckExpand);

            m_Tiles = m_MapContainer.GetTiles();
            SpawnTileMap(mapIndex);
            GameFlowManager.Instance.OnInitiateObjects.Invoke();
        }

        private void SpawnTileMap(int mapIndex)
        {
            // TODO register tile to domainManager
            _mapIndex = mapIndex;
            foreach (var tile in m_Tiles)
            {
                GameFlowManager.Instance.OnUpdateTilePos.Invoke(tile.position);
            }
            
            // SpiralPatternConstructor(tileAmount);
            //
            // // Spawn tiles
            // for (int i = 0; i < _totalTile; i++)
            // {
            //     var tile = _tilePool.GetObject();
            //     tile.transform.position = _listTilePos[i].GetPosition(0f, 1f);
            //     GameFlowManager.Instance.OnUpdateTilePos.Invoke(tile.transform.position);
            //     tile.SetActive(true);
            // }
        }

        private void SpiralPatternConstructor(int tileAmount)
        {
            _mapIndex = tileAmount;
            var spiralSpace = Mathf.RoundToInt(Mathf.Sqrt(_mapIndex)) + 1;

            int printValue = 0;
            int c1 = 0, c2 = 1;
            while (printValue < spiralSpace * spiralSpace)
            {
                //Right Direction Move  
                for (int i = c1 + 1; i <= c2; i++)
                    _listTilePos.Add(new TilePosition(c1, i, printValue++));
                //Up Direction Move  
                for (int j = c1 + 1; j <= c2; j++)
                    _listTilePos.Add(new TilePosition(j, c2, printValue++));
                //Left Direction Move  
                for (int i = c2 - 1; i >= c1; i--)
                    _listTilePos.Add(new TilePosition(c2, i, printValue++));
                //Down Direction Move  
                for (int j = c2 - 1; j >= c1; j--)
                    _listTilePos.Add(new TilePosition(j, c1, printValue++));
                c1--;
                c2++;
            }
        }

        // private void CheckExpand()
        // {
        //     var currentScore = SavingSystemManager.Instance.CalculateEnvScore();
        //
        //     foreach (var point in _expandPoints)
        //     {
        //         if (point.CheckToExpand(_totalTile, currentScore))
        //         {
        //             SpiralPatternConstructor(_totalTile + 1);
        //             var tile = _tilePool.GetObject();
        //             tile.transform.position = _listTilePos[_totalTile - 1].GetPosition(0f, 1f);
        //             GameFlowManager.Instance.OnUpdateTilePos.Invoke(tile.transform.position);
        //             tile.SetActive(true);
        //
        //             // Save environment data after expand
        //             SavingSystemManager.Instance.GetEnvironmentData().mapSize = _totalTile;
        //             SavingSystemManager.Instance.OnSavePlayerEnvData.Invoke();
        //             break;
        //         }
        //     }
        // }

        public void Reset()
        {
            _tilePool.ResetPool();
            _listTilePos = new();
        }

        public List<Transform> GetTiles()
        {
            return m_Tiles;
        }
    }
}