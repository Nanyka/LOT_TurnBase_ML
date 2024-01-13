using UnityEngine;

namespace JumpeeIsland
{
    
    
    public class AoeTutorialFactoryComp : MonoBehaviour, IInputExecutor
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
            TimelineManager.Instance.OnStringCheck.Invoke(_stringToCheck);
        }
    }
}