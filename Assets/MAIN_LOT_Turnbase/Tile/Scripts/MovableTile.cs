using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JumpeeIsland
{
    public class MovableTile : MonoBehaviour
    {
        [SerializeField] private Renderer _tileRenderer;
        [SerializeField] private Material _disableMaterial;

        private bool isDisable;
        private Transform m_Transform;
        
        private void Start()
        {
            m_Transform = transform;
        }

        public void SetDisable(List<Vector3> enableTiles)
        {
            if (enableTiles.Count(t => Vector3.Distance(t,m_Transform.position) < 0.1f) == 0)
            {
                isDisable = true;
                var materials = _tileRenderer.materials;
                materials[1] = _disableMaterial;
                _tileRenderer.materials = materials;
            }
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

        public bool IsDisable()
        {
            return isDisable;
        }
    }
}