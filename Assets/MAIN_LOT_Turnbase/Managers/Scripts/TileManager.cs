using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class TileManager : MonoBehaviour
    {
        [SerializeField] private ObjectPool _tilePool;

        [FormerlySerializedAs("_totalTile")] 
        [SerializeField] private int _mapIndex;

        [SerializeField] private MapContainer m_MapContainer;

        private List<MovableTile> m_Tiles;
        // private List<TilePosition> _listTilePos = new();

        public void Init(int mapIndex)
        {
            if (GameFlowManager.Instance.GameMode == GameMode.BOSS)
                m_MapContainer.SetDisableTile();
            m_Tiles = m_MapContainer.GetTiles();
            SpawnTileMap(mapIndex);
            GameFlowManager.Instance.OnInitiateObjects.Invoke();
        }

        private void SpawnTileMap(int mapIndex)
        {
            _mapIndex = mapIndex;
            foreach (var tile in m_Tiles)
            {
                if (tile.IsDisable())
                    continue;
                GameFlowManager.Instance.OnUpdateTilePos.Invoke(tile);
            }

            foreach (var obstacle in m_MapContainer.GetObstacles())
            {
                GameFlowManager.Instance.OnDomainRegister.Invoke(obstacle, FactionType.Neutral);
            }
        }

        // private void SpiralPatternConstructor(int tileAmount)
        // {
        //     _mapIndex = tileAmount;
        //     var spiralSpace = Mathf.RoundToInt(Mathf.Sqrt(_mapIndex)) + 1;
        //
        //     int printValue = 0;
        //     int c1 = 0, c2 = 1;
        //     while (printValue < spiralSpace * spiralSpace)
        //     {
        //         //Right Direction Move  
        //         for (int i = c1 + 1; i <= c2; i++)
        //             _listTilePos.Add(new TilePosition(c1, i, printValue++));
        //         //Up Direction Move  
        //         for (int j = c1 + 1; j <= c2; j++)
        //             _listTilePos.Add(new TilePosition(j, c2, printValue++));
        //         //Left Direction Move  
        //         for (int i = c2 - 1; i >= c1; i--)
        //             _listTilePos.Add(new TilePosition(c2, i, printValue++));
        //         //Down Direction Move  
        //         for (int j = c2 - 1; j >= c1; j--)
        //             _listTilePos.Add(new TilePosition(j, c1, printValue++));
        //         c1--;
        //         c2++;
        //     }
        // }

        public void Reset()
        {
            _tilePool.ResetPool();
            // _listTilePos = new();
        }

        // public List<Transform> GetTiles()
        // {
        //     return m_Tiles;
        // }
    }
}