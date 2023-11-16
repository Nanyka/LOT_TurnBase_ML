using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class TowerComp : MonoBehaviour, ICheckableObject
    {
        [SerializeField] private string m_State;
        
        private bool IsAvailable { get; set; }

        #region INITIATE
        
        private void OnEnable()
        {
            Init();
        }

        private void Init()
        {
            Invoke(nameof(TempSetWorldState),1f);
        }

        //TODO: refactor --> call for initiating GWorld first and not use this Invoke
        private void TempSetWorldState()
        {
            IsAvailable = true;
            GWorld.Instance.GetWorld().ModifyState(m_State, 1);
        }

        private void OnDisable()
        {
            Destroyed();
        }
        
        private void Destroyed()
        {
            if (IsAvailable && GWorld.Instance != null)
            {
                GWorld.Instance.GetWorld().ModifyState(m_State, -1);
                IsAvailable = false;
            }
        }

        #endregion

        #region CHECKABLE FEATUREs

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
            
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        #endregion
    }
}