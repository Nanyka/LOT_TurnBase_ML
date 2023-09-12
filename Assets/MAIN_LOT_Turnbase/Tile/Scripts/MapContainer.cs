using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class MapContainer : MonoBehaviour
    {
        [SerializeField] private List<MovableTile> m_MovableTiles;
        [SerializeField] private List<GameObject> m_Obstacle;

        public void SetDisableTile()
        {
            var enablePos = GameFlowManager.Instance.GetQuest().enableTiles;
            foreach (var tile in m_MovableTiles)
            {
                tile.SetDisable(enablePos);
            }
        }
        
        public List<MovableTile> GetTiles()
        {
            return m_MovableTiles;
        }
        
        public List<GameObject> GetObstacles()
        {
            return m_Obstacle;
        }
    }
}
