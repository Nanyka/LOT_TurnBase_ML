using UnityEngine;

namespace JumpeeIsland
{
    public interface IInputExecutor
    {
        public void OnClick();
        public void OnHoldEnter();
        public void OnHolding(Vector3 position);
        public void OnHoldCanCel();
        public void OnDoubleTaps();
    }
}