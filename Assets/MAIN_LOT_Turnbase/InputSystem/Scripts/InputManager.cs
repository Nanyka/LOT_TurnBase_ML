using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class InputManager : Singleton<InputManager>
    {
        [SerializeField] private LayerMask _selectLayer;
        [SerializeField] private LayerMask _dropLayer;
        // [SerializeField] private LayerMask _assignLayer;

        private IInputExecutor _currentExecutor;
        private IGlobalInteract _globalInteract;
        private Camera _mainCamera;
        private bool _isDropEntity;
        private bool _isCameraMove;

        private void Start()
        {
            _globalInteract = GetComponent<IGlobalInteract>();
            _mainCamera = Camera.main;
        }

        public void PlayerClick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (PointingChecker.IsPointerOverUIObject())
                    return;

                var moveRay = _mainCamera.ScreenPointToRay(Touchscreen.current.primaryTouch.position.ReadValue());
                if (Physics.Raycast(moveRay, out var selectHit, 100f, _selectLayer))
                {   
                    if (selectHit.collider.TryGetComponent(out IInputExecutor executor))
                    {
                        _currentExecutor = executor;
                        _currentExecutor.OnClick();
                    }
                }
                else
                {
                    MainUI.Instance.OnHideAllMenu.Invoke();
                }
            }
        }

        public void PlayerHold(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (PointingChecker.IsPointerOverUIObject())
                    return;

                var moveRay = _mainCamera.ScreenPointToRay(Touchscreen.current.primaryTouch.position.ReadValue());
                if (Physics.Raycast(moveRay, out var selectHit, 100f, _selectLayer))
                {
                    _isDropEntity = true;
                    
                    if (selectHit.collider.TryGetComponent(out IInputExecutor executor))
                    {
                        _currentExecutor = executor;
                        _currentExecutor.OnHoldEnter();
                    }
                }
                else
                {
                    _isCameraMove = true;
                }
            }

            if (context.canceled)
            {
                _isDropEntity = false;
                _isCameraMove = false;

                if (_currentExecutor != null)
                {
                    _currentExecutor.OnHoldCanCel();
                    _currentExecutor = null;
                }
            }
        }

        public void PlayerDoubleTap(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (PointingChecker.IsPointerOverUIObject())
                    return;

                var moveRay = _mainCamera.ScreenPointToRay(Touchscreen.current.primaryTouch.position.ReadValue());
                if (Physics.Raycast(moveRay, out var selectHit, 100f, _selectLayer))
                {   
                    if (selectHit.collider.TryGetComponent(out IInputExecutor executor))
                    {
                        _currentExecutor = executor;
                        _currentExecutor.OnDoubleTaps();
                    }
                }
                else if (Physics.Raycast(moveRay, out var assignHit, 100f, _dropLayer))
                {   
                    _globalInteract.OnDoubleTap(assignHit.point);
                }
            }
        }

        public void PlayerDetectTouch(InputAction.CallbackContext context)
        {
            // TODO Select object from selectLayer and drop point at dropLayer
            if (_isDropEntity)
            {
                if (PointingChecker.IsPointerOverUIObject())
                    return;

                var moveRay = _mainCamera.ScreenPointToRay(context.ReadValue<Vector2>());
                if (!Physics.Raycast(moveRay, out var moveHit, 100f, _dropLayer)) return;
                _currentExecutor?.OnHolding(moveHit.point);
            }
        }

        public void PlayerDetectDelta(InputAction.CallbackContext context)
        {
            if (_isCameraMove)
                CameraController.Instance.OnMoveFocalPoint.Invoke(context.ReadValue<Vector2>());
        }

        public Vector3 GetTouchPoint()
        {
            var moveRay = _mainCamera.ScreenPointToRay(Touchscreen.current.primaryTouch.position.ReadValue());
            if (Physics.Raycast(moveRay, out var dropHit, 100f, _dropLayer))
                return dropHit.point;
            
            return Vector3.zero;
        }
    }
}