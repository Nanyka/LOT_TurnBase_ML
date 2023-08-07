using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JumpeeIsland
{
    public class RelocateComp : MonoBehaviour
    {
        private Entity m_Entity;
        private Camera _camera;
        private int _buildingLayer = 1 << 9;
        private int _tileLayer = 1 << 6;
        private bool _isSelectEntity;

        private void Start()
        {
            _camera = Camera.main;
            m_Entity = GetComponent<Entity>();
        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (m_Entity.GetFaction() != FactionType.Player)
                    return;

                if (MainUI.Instance.IsInteractable == false || PointingChecker.IsPointerOverUIObject())
                    return;

                var moveRay = _camera.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(moveRay, out var moveHit, 100f, _buildingLayer))
                    return;

                _isSelectEntity = true;
            }

            if (Input.GetMouseButton(0))
            {
                if (_isSelectEntity == false)
                    return;
                
                var ray = _camera.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out var collidedTile, 100f, _tileLayer))
                    return;
                
                m_Entity.Relocate(collidedTile.transform.position);
            }

            if (Input.GetMouseButtonUp(0))
            {
                _isSelectEntity = false;
                var mTransform = transform;
                m_Entity.UpdateTransform(mTransform.position,mTransform.eulerAngles);
            }
        }
    }
}