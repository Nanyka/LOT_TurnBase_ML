using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class BattleEcoUI : MainUI
    {
        private AoeWorkerMenu _dropWorker;
        private AoeConstructionMenu _dropConstruction;
        
        protected override void Start()
        {
            _dropWorker = GetComponent<AoeWorkerMenu>();
            _dropConstruction = GetComponent<AoeConstructionMenu>();
            _mainCamera = Camera.main;

            GameFlowManager.Instance.OnStartGame.AddListener(EnableInteract);
        }
        
        protected override void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (PointingChecker.IsPointerOverUIObject() || _dropWorker._isInADeal || _dropConstruction._isInADeal)
                    return;

                var moveRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(moveRay, out var moveHit, 100f, _layerMask))
                {
                    if (moveHit.collider.TryGetComponent(out SelectionCircle selectionCircle))
                        OnSelectDirection.Invoke(selectionCircle);
                        
                    return;
                }
                
                OnHideAllMenu.Invoke();
            }
        }
    }
}
