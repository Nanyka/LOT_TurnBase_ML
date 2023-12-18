using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JumpeeIsland
{
    public class BuildingInteractComp : MonoBehaviour, IInputExecutor, IShowInfo, IConfirmFunction
    {
        private Entity m_Entity;
        private bool _isOnRelocating;

        private void Start()
        {
            m_Entity = GetComponent<Entity>();
        }

        public void OnClick()
        {
            MainUI.Instance.OnShowInfo.Invoke(this);
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
            if (_isOnRelocating == false)
                return;
            
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

        public (Entity entity, int jump) ShowInfo()
        {
            return (m_Entity, 0);
        }
        
        public void ClickYes()
        {
            Debug.Log("No sellable object");
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}