using System;
using GOAP;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class BuildingConstructComp : MonoBehaviour, ICheckableObject, IChangeWorldState, IBuildingConstruct
    {
        [NonSerialized] public UnityEvent _completed = new();
        
        [SerializeField] private int _cost;
        [SerializeField] private string m_InProcessState;
        [SerializeField] private string m_FinishState;
        
        private bool _isFinishConstructed;
        private int _curProcess;
        private bool IsAvailable { get; set; }
        private bool _isInit;

        public void Init(FactionType factionType)
        {
            _isInit = true;
            _isFinishConstructed = factionType != FactionType.Player;
            
            if (_isFinishConstructed)
                Completion();
            else
            {
                _curProcess = 0;
                SetResourceScale();
                GWorld.Instance.GetWorld().ModifyState(m_InProcessState, 1);
            }
            
        }

        private void OnDisable()
        {
            if (GWorld.Instance != null && _isInit)
                GWorld.Instance.GetWorld().ModifyState(_isFinishConstructed ? m_FinishState : m_InProcessState, -1);
        }

        private void Completion()
        {
            if (IsAvailable == false)
            {
                GWorld.Instance.GetWorld().ModifyState(m_FinishState, 1);
                GWorld.Instance.GetWorld().ModifyState(m_InProcessState, -1);
                _completed.Invoke();
                IsAvailable = true;
            }
        }

        private void Destroyed()
        {
            if (IsAvailable && GWorld.Instance != null)
            {
                _curProcess = 0;
                SetResourceScale();
                GWorld.Instance.GetWorld().ModifyState(m_FinishState, -1);
                IsAvailable = false;
            }
        }

        #region EXPLOITING

        public bool IsCheckable()
        {
            return IsAvailable;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public void ReduceCheckableAmount(int amount)
        {
            if (IsAvailable)
                return;

            _curProcess = Mathf.Clamp(_curProcess + amount, 0, _cost);
            SetResourceScale();
            if (_curProcess >= _cost)
            {
                _isFinishConstructed = true;
                Completion();
            }
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public int GetRemainAmount()
        {
            return _curProcess;
        }

        private void SetResourceScale()
        {
            var mTransform = transform;
            var localScale = mTransform.localScale;
            localScale = new Vector3(localScale.x, _curProcess * 1f / _cost, localScale.z);
            mTransform.localScale = localScale;
        }

        #endregion

        public void ChangeState(int amount)
        {
            GWorld.Instance.GetWorld().ModifyState(m_FinishState, amount);
        }

        public UnityEvent GetCompletedEvent()
        {
            return _completed;
        }
    }
    
    public interface IBuildingConstruct
    {
        public UnityEvent GetCompletedEvent();
        
    }
}