using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class MovableTile : MonoBehaviour
    {
        private Transform m_Transform;
        
        private void Start()
        {
            m_Transform = transform;
        }

        public Vector3 GetPosition()
        {
            return m_Transform.position;
        }

        public bool CheckGeoCoordinates(Vector3 checkPos)
        {
            var curPos = GetPosition();
            return Math.Abs(curPos.x - checkPos.x) < 0.1f && Math.Abs(curPos.z - checkPos.z) < 0.1f;
        }
    }
}