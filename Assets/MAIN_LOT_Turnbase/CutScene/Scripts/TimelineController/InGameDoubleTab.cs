using UnityEngine;

namespace JumpeeIsland
{
    public class InGameDoubleTab : MonoBehaviour, IInputExecutor
    {
        [SerializeField] private string _stringToCheck;

        public void OnClick()
        {
            
        }

        public void OnHoldEnter()
        {
            
        }

        public void OnHolding(Vector3 position)
        {
            
        }

        public void OnHoldCanCel()
        {
            
        }

        public void OnDoubleTaps()
        {
            Debug.Log($"Double tap on {name}");
            GameFlowManager.Instance.AskGlobalVfx(GlobalVfxType.RADAR, transform.position);
            TimelineManager.Instance.OnStringCheck.Invoke(_stringToCheck);
        }
    }
}