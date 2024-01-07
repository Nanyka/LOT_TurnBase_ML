using System;
using System.Collections;
using GOAP;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class BuildingConstructComp : MonoBehaviour, ICheckableObject, IChangeWorldState, IBuildingConstruct
    {
        [NonSerialized] private readonly UnityEvent _completed = new();
        [SerializeField] private int _cost;
        [SerializeField] private string m_InProcessState;
        [SerializeField] private string m_FinishState;
        [SerializeField] private bool _isSelfErect;

        private IEntityUIUpdate _healthBarUpdate;
        private bool _isFinishConstructed;
        private int _curProcess;
        private bool IsAvailable;
        private bool _isInit;

        private void Awake()
        {
            _healthBarUpdate = GetComponent<IEntityUIUpdate>();
        }

        public void Init(FactionType factionType)
        {
            _isInit = true;
            _isFinishConstructed = factionType != FactionType.Player;
            IsAvailable = false;
            
            if (_isFinishConstructed)
                Completion();
            else
            {
                _curProcess = 0;
                SetResourceScale();
                GWorld.Instance.GetWorld().ModifyState(m_InProcessState, 1);

                if (_isSelfErect)
                    InvokeRepeating(nameof(SelfErect),1f,1f);
            }
        }

        private void SelfErect()
        {
            ReduceCheckableAmount(1);
            _healthBarUpdate.UpdateHealthSlider(_curProcess*1f/_cost);
            if (IsAvailable || _isFinishConstructed)
                CancelInvoke();
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

        #region EXPLOITING

        public bool IsCheckable()
        {
            return IsAvailable && gameObject.activeInHierarchy;
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

        public IChangeWorldState GetWorldStateChanger()
        {
            return this;
        }
    }
    
    public interface IBuildingConstruct
    {
        public void Init(FactionType factionType);
        public UnityEvent GetCompletedEvent();
        public IChangeWorldState GetWorldStateChanger();
    }
}