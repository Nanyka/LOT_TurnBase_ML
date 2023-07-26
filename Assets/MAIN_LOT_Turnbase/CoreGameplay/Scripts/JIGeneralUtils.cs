using UnityEngine;

namespace JumpeeIsland
{
    public class JIGeneralUtils
    {
        public static Vector3 DirectionTo(int direction)
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
    }
}