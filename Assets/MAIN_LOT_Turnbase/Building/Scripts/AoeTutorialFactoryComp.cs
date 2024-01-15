using UnityEngine;

namespace JumpeeIsland
{
    
    
    public class AoeTutorialFactoryComp : MonoBehaviour, IInputExecutor
    {
        [SerializeField] private string _stringToCheck;
        // [SerializeField] private ButtonRequire _buttonRequire;
        
        private bool _isOnHolding;
        
        private void OnDisable()
        {
            _isOnHolding = false;
        }

        public void OnClick()
        {
            TimelineManager.Instance.OnStringCheck.Invoke(_stringToCheck, ButtonRequire.CLICK);
        }

        public void OnHoldEnter()
        {
            _isOnHolding = true;
        }

        public void OnHolding(Vector3 position)
        {
            
        }

        public void OnHoldCanCel()
        {
            if (_isOnHolding == false)
                return;
                
            _isOnHolding = false;
            TimelineManager.Instance.OnStringCheck.Invoke(_stringToCheck, ButtonRequire.HOLD);
        }

        public void OnDoubleTaps()
        {
            TimelineManager.Instance.OnStringCheck.Invoke(_stringToCheck, ButtonRequire.DOUBLETAP);
        }
    }
}