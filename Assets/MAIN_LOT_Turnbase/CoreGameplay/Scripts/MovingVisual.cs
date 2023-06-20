using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class MovingVisual : MonoBehaviour
    {
        [SerializeField] protected SelectionCircle[] _movingPoints;
        [SerializeField] private Material[] _attackingColors;

        private EnvironmentManager m_Environment;
        private Camera _camera;
        private int _layerMask = 1 << 8;

        private void Awake()
        {
            m_Environment = GetComponent<EnvironmentManager>();
        }

        private void Start()
        {
            m_Environment.OnShowMovingPath.AddListener(MovingRange);
            m_Environment.OnHighlightUnit.AddListener(HighlightUnit);
            MainUI.Instance.OnClickIdleButton.AddListener(DisableMovingPath);

            _camera = Camera.main;
        }

        private void MovingRange(Vector3 middlePos)
        {
            for (int index = 0; index <= 3; index++)
            {
                _movingPoints[index].SwitchProjector(false);

                var currentMovement = m_Environment.GetMovementInspector()
                    .MovingPath(middlePos, index + 1, 0, 0); // use "index+1" to show just moving directions

                if (Vector3.Distance(currentMovement.returnPos, middlePos) < Mathf.Epsilon)
                    continue;

                _movingPoints[index].SwitchProjector(currentMovement.returnPos, index + 1);
            }
        }

        private void HighlightUnit(Vector3 position)
        {
            DisableMovingPath();
            _movingPoints[0].HighlightAt(position);
        }

        #region INTERACT SELECTION

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var moveRay = _camera.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(moveRay, out var moveHit, 100f, _layerMask))
                    return;

                if (!moveHit.collider.TryGetComponent(out SelectionCircle selectionCircle)) return;
                m_Environment.OnTouchSelection.Invoke(selectionCircle.GetDirection());
                DisableMovingPath();
            }
        }

        private void DisableMovingPath()
        {
            foreach (var circle in _movingPoints)
                circle.SwitchProjector(false);
        }

        #endregion
    }
}