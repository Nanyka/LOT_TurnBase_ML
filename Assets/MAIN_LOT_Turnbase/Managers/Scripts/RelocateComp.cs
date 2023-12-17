using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JumpeeIsland
{
    public class RelocateComp : MonoBehaviour, IInputExecutor
    {
        // private Entity m_Entity;
        // private Camera _camera;
        // private int _buildingLayer = 1 << 9;
        // private int _tileLayer = 1 << 6;
        // private bool _isSelectEntity;
        //
        // private void Start()
        // {
        //     _camera = Camera.main;
        //     m_Entity = GetComponent<Entity>();
        // }
        //
        // public void Update()
        // {
        //     if (Input.GetMouseButtonDown(0))
        //     {
        //         if (m_Entity.GetFaction() != FactionType.Player)
        //             return;
        //
        //         if (MainUI.Instance.IsInteractable == false || PointingChecker.IsPointerOverUIObject())
        //             return;
        //
        //         var moveRay = _camera.ScreenPointToRay(Input.mousePosition);
        //         if (!Physics.Raycast(moveRay, out var moveHit, 100f, _buildingLayer))
        //             return;
        //
        //         if (moveHit.transform == transform)
        //         {
        //             _isSelectEntity = true;
        //             MainUI.Instance.IsInRelocating = _isSelectEntity;
        //         }
        //     }
        //
        //     if (Input.GetMouseButton(0))
        //     {
        //         if (_isSelectEntity == false)
        //             return;
        //
        //         var ray = _camera.ScreenPointToRay(Input.mousePosition);
        //         if (!Physics.Raycast(ray, out var collidedTile, 100f, _tileLayer))
        //             return;
        //
        //         m_Entity.Relocate(collidedTile.transform.position);
        //     }
        //
        //     if (Input.GetMouseButtonUp(0))
        //     {
        //         if (_isSelectEntity == false)
        //             return;
        //         
        //         _isSelectEntity = false;
        //         MainUI.Instance.IsInRelocating = _isSelectEntity;
        //         var mTransform = transform;
        //         m_Entity.UpdateTransform(mTransform.position, mTransform.eulerAngles);
        //     }
        // }

        private Entity m_Entity;
        private bool _isOnRelocating;

        private void Start()
        {
            m_Entity = GetComponent<Entity>();
        }

        public void OnClick()
        {
            // Debug.Log($"On clicking {name}");
        }

        public void OnHoldEnter()
        {
            if (GameFlowManager.Instance.GameMode == GameMode.AOE)
                return;
            
            if (m_Entity.GetFaction() != FactionType.Player)
                return;

            if (MainUI.Instance.IsInteractable == false || PointingChecker.IsPointerOverUIObject())
                return;

            _isOnRelocating = true;
            MainUI.Instance.IsInRelocating = _isOnRelocating;

            // Debug.Log($"On holding {name}");
        }

        public void OnHolding(Vector3 position)
        {
            position = new Vector3(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));
            m_Entity.Relocate(position);
        }

        public void OnHoldCanCel()
        {
            if (_isOnRelocating == false)
                return;
            
            _isOnRelocating = false;
            MainUI.Instance.IsInRelocating = _isOnRelocating;
            var mTransform = transform;
            m_Entity.UpdateTransform(mTransform.position, mTransform.eulerAngles);
            // Debug.Log($"On cancelling {name}");
        }

        public void OnDoubleTaps()
        {
            // Debug.Log($"On double tapping {name}");
        }
    }
}