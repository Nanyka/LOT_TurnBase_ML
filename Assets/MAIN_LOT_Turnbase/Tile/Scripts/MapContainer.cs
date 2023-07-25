using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class MapContainer : MonoBehaviour
    {
        [SerializeField] private List<MovableTile> m_MovableTiles;

        public List<MovableTile> GetTiles()
        {
            return m_MovableTiles;
        }
    }
}
