using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class MovingVisual : MonoBehaviour
    {
        [SerializeField] protected SelectionCircle[] _movingPoints;

        private EnvironmentManager m_Environment;

        private void Awake()
        {
            m_Environment = GetComponent<EnvironmentManager>();
        }

        private void Start()
        {
            m_Environment.OnShowMovingPath.AddListener(MovingRange);
            m_Environment.OnHighlightUnit.AddListener(HighlightUnit);
            MainUI.Instance.OnClickIdleButton.AddListener(DisableMovingPath);
            MainUI.Instance.OnSelectDirection.AddListener(SelectDirection);
        }

        private void MovingRange(Vector3 middlePos)
        {
            middlePos = new Vector3(Mathf.RoundToInt(middlePos.x),
                Mathf.RoundToInt(middlePos.y), Mathf.RoundToInt(middlePos.z));

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

        private void SelectDirection(SelectionCircle selectionCircle)
        {
            m_Environment.OnTouchSelection.Invoke(selectionCircle.GetDirection());
            DisableMovingPath();
        }

        public void DisableMovingPath()
        {
            foreach (var circle in _movingPoints)
                circle.SwitchProjector(false);
        }

        #endregion
    }
}