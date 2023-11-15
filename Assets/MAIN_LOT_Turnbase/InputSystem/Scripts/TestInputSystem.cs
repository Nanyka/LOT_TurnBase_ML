using UnityEngine;
using UnityEngine.InputSystem;

namespace JumpeeIsland
{
    public class TestInputSystem : MonoBehaviour
    {
        public void PlayerClick(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
                Debug.Log("Player click");
        }
        
        public void PlayerHold(InputAction.CallbackContext context)
        {
            if (context.performed)
                Debug.Log("Player hold");
        }

        public void PlayerDetectTouch(InputAction.CallbackContext context)
        {
            Debug.Log(context.ReadValue<Vector2>());
        }
    }
}