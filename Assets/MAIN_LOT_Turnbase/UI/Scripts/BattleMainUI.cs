using System;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class BattleMainUI : MainUI
    {
        protected override void Start()
        {
            // _creatureMenu = GetComponent<CreatureMenu>();
            // _mainCamera = Camera.main;

            OnEnableInteract.AddListener(EnableInteractable);
            GameFlowManager.Instance.OnGameOver.AddListener(DisableInteractable);
        }

        private void EnableInteractable()
        {
            IsInteractable = true;
        }

        private void DisableInteractable(int delayInterval)
        {
            IsInteractable = false;
        }

        // protected override void Update()
        // {
        //     if (Input.GetMouseButton(0))
        //     {
        //         var moveRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
        //         if (Physics.Raycast(moveRay, out var moveHit, 100f, _layerMask))
        //         {
        //             if (moveHit.collider.TryGetComponent(out SelectionCircle selectionCircle))
        //                 OnSelectDirection.Invoke(selectionCircle);
        //         }
        //     }
        // }
    }
}