using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOT_Turnbase
{
    public class TileManager : MonoBehaviour
    {
        [SerializeField] private int _freeSpaceMultiplier = 1;
        [SerializeField] private ObjectPool _tilePool;

        private int _totalTile;
        private List<TilePosition> _listTilePos = new();

        public void Init(int tileAmount)
        {
            SpawnTileMap(tileAmount);
            StartUpProcessor.Instance.OnInitiateObjects.Invoke();
        }

        private void SpawnTileMap(int tileAmount)
        {
            SpiralPatternConstructor(tileAmount);

            // Spawn tiles
            for (int i = 0; i < _totalTile; i++)
            {
                var tile = _tilePool.GetObject();
                tile.transform.position = _listTilePos[i].GetPosition(0f, 1f);
                StartUpProcessor.Instance.OnUpdateTilePos.Invoke(tile.transform.position);
                tile.SetActive(true);
            }
        }

        private void SpiralPatternConstructor(int tileAmount)
        {
            _totalTile = tileAmount * _freeSpaceMultiplier;
            var spiralSpace = Mathf.RoundToInt(Mathf.Sqrt(_totalTile)) + 1;

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
    }
}