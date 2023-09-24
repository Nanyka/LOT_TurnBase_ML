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
            if (_environment.CheckOutOfBoundary(curPos))
                return (curPos, jumpCount, overEnemy);
            
            var newPos = _environment.GetTilePosByGeoPos(curPos + DirectionTo(direction));

            if (newPos.x.CompareTo(float.NegativeInfinity) == 0 || _environment.CheckOutOfBoundary(newPos))
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
            if (newPos.x.CompareTo(float.NegativeInfinity) == 0 || _environment.CheckOutOfBoundary(newPos))
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

        public Vector3 DirectionTo(int direction)
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
        
        public Vector3 DirectionTo(int direction, Vector3 forward)
        {
            var checkVector = Vector3.zero;

            switch (direction)
            {
                case 0:
                    break;
                case 1:
                    checkVector = new Vector3(forward.z,forward.y, forward.x);
                    break;
                case 2:
                    checkVector = new Vector3(forward.z*-1f,forward.y, forward.x * -1f);
                    break;
                case 3:
                    checkVector = new Vector3(forward.x*-1f,forward.y, forward.z * -1f);
                    break;
                case 4:
                    checkVector = forward;
                    break;
            }

            return checkVector;
        }

        public int ChangeActionByDirection(Vector3 toward)
        {
            if (Vector3.Distance(toward, Vector3.left) < 0.1f)
                return 1;
            if (Vector3.Distance(toward, Vector3.right) < 0.1f)
                return 2;
            if (Vector3.Distance(toward, Vector3.back) < 0.1f)
                return 3;
            if (Vector3.Distance(toward, Vector3.forward) < 0.1f)
                return 4;
            return 0;
        }

        private bool CheckAvailableMove(Vector3 newPos)
        {
            return _environment.FreeToMove(newPos);
        }
    }
}