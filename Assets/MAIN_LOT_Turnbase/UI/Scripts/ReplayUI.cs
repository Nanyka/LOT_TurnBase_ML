using UnityEngine;

namespace JumpeeIsland
{
    public class ReplayUI : MainUI
    {
        protected override void Start()
        {
            _mainCamera = Camera.main;
            GameFlowManager.Instance.OnStartGame.AddListener(EnableInteract);
        }

        protected override void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Do something when touch on the screen");
            }
        }
    }
}