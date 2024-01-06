using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class ResourceRegenComp : MonoBehaviour, ICheckableObject
    {
        [SerializeField] private int _capacity;
        [SerializeField] private float _recoverySpeed = 1f;
        [SerializeField] private int _recoveryAmount = 1;
        [SerializeField] private string m_WorldState;

        private int _curStorage;

        private bool IsAvailable { get; set; }

        private void OnEnable()
        {
            Init();
        }

        private void OnDisable()
        {
            ModifyWorldState();
            CancelInvoke();
        }

        private void Init()
        {
            _curStorage = _capacity;
            Refresh(); // Register the resource at the beginning of the game
            // CancelInvoke();
            InvokeRepeating(nameof(Regenerate), _recoverySpeed, _recoverySpeed);
        }

        private void Refresh()
        {
            Invoke(nameof(TempSetWorldState),1f);
        }
        
        //TODO: refactor --> call for initiating GWorld first and not use this Invoke
        private void TempSetWorldState()
        {
            if (IsAvailable == false)
            {
                GWorld.Instance?.GetWorld().ModifyState(m_WorldState, 1);
                IsAvailable = true;
            }
        }

        private void ModifyWorldState()
        {
            if (IsAvailable && GWorld.Instance != null)
            {
                GWorld.Instance.GetWorld().ModifyState(m_WorldState, -1);
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
            _curStorage = Mathf.Clamp(_curStorage - amount, 0, _curStorage - amount);
            SetResourceScale();
            if (_curStorage <= 0)
                ModifyWorldState();
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public int GetRemainAmount()
        {
            return _curStorage;
        }

        private void Regenerate()
        {
            _curStorage = Mathf.Clamp(_curStorage + _recoveryAmount, 0, _capacity);
            SetResourceScale();
            if (IsAvailable == false && _curStorage > 0)
                Refresh();
        }

        private void SetResourceScale()
        {
            var mTransform = transform;
            var localScale = mTransform.localScale;
            localScale = new Vector3(localScale.x, _curStorage * 1f / _capacity, localScale.z);
            mTransform.localScale = localScale;
        }

        #endregion
    }
}