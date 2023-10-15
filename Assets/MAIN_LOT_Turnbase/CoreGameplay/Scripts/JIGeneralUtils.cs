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
        
        public static Vector3 AdverseDirectionTo(int direction)
        {
            var checkVector = Vector3.zero;

            switch (direction)
            {
                case 1:
                    checkVector += Vector3.right;
                    break;
                case 2:
                    checkVector += Vector3.left;
                    break;
                case 3:
                    checkVector += Vector3.forward;
                    break;
                case 4:
                    checkVector += Vector3.back;
                    break;
            }

            return checkVector;
        }

        public static Vector3 RotateTo(int direction)
        {

            switch (direction)
            {
                case 1:
                    return new Vector3(0f, -90f, 0f);
                case 2:
                    return new Vector3(0f, 90f, 0f);
                    break;
                case 3:
                    return new Vector3(0f, 180f, 0f);
                case 4:
                    return Vector3.zero;
            }
            return Vector3.zero;
        }
        
        public static Vector3 AdverseRotateTo(int direction)
        {

            switch (direction)
            {
                case 1:
                    return new Vector3(0f, 90f, 0f);
                case 2:
                    return new Vector3(0f, -90f, 0f);
                    break;
                case 3:
                    return Vector3.zero;
                case 4:
                    return new Vector3(0f, 180f, 0f);
            }
            return Vector3.zero;
        }
    }
}