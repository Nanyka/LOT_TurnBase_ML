using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class MapContainer : MonoBehaviour
    {
        [SerializeField] private List<Transform> m_MovableTiles;

        public List<Transform> GetTiles()
        {
            return m_MovableTiles;
        }
    }
}
