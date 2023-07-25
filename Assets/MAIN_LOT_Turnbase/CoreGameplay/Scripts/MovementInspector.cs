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
            var newPos = _environment.GetTilePosByGeoPos(curPos + DirectionTo(direction));

            if (newPos == Vector3.negativeInfinity || _environment.CheckOutOfBoundary(newPos))
                return (curPos, jumpCount, overEnemy);

            if (CheckAvailableMove(newPos))
            {
                if (jumpCount == 0)
                {
                    // If newPos.y is different from curPos.y
                    // jumpCount increase and update curPos to go to the new loop
                    // but if newPos and curPos is similar in velocity
                    // return newPos

                    if (_environment.CheckTileHeight(curPos, newPos))
                        return (newPos, jumpCount, overEnemy);
                    
                    jumpCount++;
                    curPos = newPos;
                    return MovingPath(curPos, direction, jumpCount, overEnemy);
                }

                return (curPos, jumpCount, overEnemy);
            }

            if (_environment.CheckHigherTile(curPos,newPos))
                return (curPos, jumpCount, overEnemy);

            if (CheckAvailableMove(newPos + DirectionTo(direction)))
            {
                jumpCount++;
                curPos = _environment.GetTilePosByGeoPos(newPos + DirectionTo(direction));
                return MovingPath(curPos, direction, jumpCount, overEnemy);
            }

            return (curPos, jumpCount, overEnemy);
        }

        // Get list of jumping positions
        public List<Vector3> MovingPath(Vector3 curPos, int direction, List<Vector3> jumpingPoints)
        {
            var newPos = _environment.GetTilePosByGeoPos(curPos + DirectionTo(direction));
            if (newPos == Vector3.negativeInfinity || _environment.CheckOutOfBoundary(newPos))
                return jumpingPoints;
            
            if (CheckAvailableMove(newPos))
            {
                if (jumpingPoints.Count == 0)
                {
                    jumpingPoints.Add(newPos);
                    if (_environment.CheckTileHeight(curPos, newPos) == false)
                    {
                        curPos = newPos;
                        return MovingPath(curPos, direction, jumpingPoints);
                    }
                }
                return jumpingPoints;
            }
            
            if (_environment.CheckHigherTile(curPos,newPos))
                return jumpingPoints;

            if (CheckAvailableMove(newPos + DirectionTo(direction)))
            {
                curPos = _environment.GetTilePosByGeoPos(newPos + DirectionTo(direction));
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