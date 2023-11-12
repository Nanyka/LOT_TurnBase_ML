using GOAP;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class BuildingConstructComp : MonoBehaviour, ICheckableObject
    {
        [SerializeField] private int _cost;
        [SerializeField] private string m_InProcessState;
        [SerializeField] private string m_FinishState;
        [SerializeField] private bool _isFinishConstructed;

        private int _curProcess;

        public bool IsAvailable { get; private set; }

        private void OnEnable()
        {
            Init();
        }

        private void Init()
        {
            if (_isFinishConstructed)
                Refresh();
            else
            {
                _curProcess = 0;
                SetResourceScale();
                GWorld.Instance.GetWorld().ModifyState(m_InProcessState, 1);
            }
        }

        private void OnDisable()
        {
            Destroyed();
        }

        private void Refresh()
        {
            GWorld.Instance.GetWorld().ModifyState(m_FinishState, 1);
            GWorld.Instance.GetWorld().ModifyState(m_InProcessState, -1);

            IsAvailable = true;
        }

        private void Destroyed()
        {
            _curProcess = 0;
            SetResourceScale();
            GWorld.Instance.GetWorld().ModifyState(m_FinishState, -1);
            IsAvailable = false;
        }

        #region EXPLOITING

        public bool IsCheckable()
        {
            return !IsAvailable;
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
                Refresh();
        }

        private void SetResourceScale()
        {
            var mTransform = transform;
            var localScale = mTransform.localScale;
            localScale = new Vector3(localScale.x, _curProcess * 1f / _cost, localScale.z);
            mTransform.localScale = localScale;
        }

        #endregion
    }
}