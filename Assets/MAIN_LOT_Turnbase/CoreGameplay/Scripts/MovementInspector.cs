using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class MovementInspector : MonoBehaviour
    {
        private EnvironmentManager _environment;

        private void Awake()
        {
            _environment = GetComponent<EnvironmentManager>();
        }

        public (Vector3 returnPos, int jumpCount, int overEnemy) MovingPath(Vector3 curPos, int direction,
            int jumpCount, int overEnemy)
        {
            var newPos = curPos + DirectionTo(direction);

            if (CheckAvailableMove(newPos))
            {
                if (jumpCount == 0)
                    return (newPos, jumpCount, overEnemy);

                return (curPos, jumpCount, overEnemy);
            }

            if (CheckAvailableMove(newPos + DirectionTo(direction)))
            {
                jumpCount++;
                curPos = newPos + DirectionTo(direction);
                return MovingPath(curPos, direction, jumpCount, overEnemy);
            }

            return (curPos, jumpCount, overEnemy);
        }

        // Get list of jumping positions
        public List<Vector3> MovingPath(Vector3 curPos, int direction, List<Vector3> jumpingPoints)
        {
            var newPos = curPos + DirectionTo(direction);

            if (CheckAvailableMove(newPos))
            {
                if (jumpingPoints.Count == 0)
                    jumpingPoints.Add(newPos);
                return jumpingPoints;
            }

            if (CheckAvailableMove(newPos + DirectionTo(direction)))
            {
                curPos = newPos + DirectionTo(direction);
                jumpingPoints.Add(curPos);
                return MovingPath(curPos, direction, jumpingPoints);
            }

            return jumpingPoints;
        }

        private Vector3 DirectionTo(int direction)
        {
            var checkVector = Vector3.zero;

            switch (direction)
            {
                case 0:
                    break;
                case 1:
                    checkVector += Vector3.left;
                    break;
                case 2:
                    checkVector += Vector3.right;
                    break;
                case 3:
                    checkVector += Vector3.back;
                    break;
                case 4:
                    checkVector += Vector3.forward;
                    break;
            }

            return checkVector;
        }

        private bool CheckAvailableMove(Vector3 newPos)
        {
            return _environment.FreeToMove(newPos);
        }
    }
}