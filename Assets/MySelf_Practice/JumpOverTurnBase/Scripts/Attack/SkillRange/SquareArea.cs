using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareArea : SkillRange
{
    private List<Vector3> range = new();
    
    public Vector3 InvokeAt(Vector3 currentPos, Vector3 direction)
    {
        return currentPos;
    }

    public IEnumerable<Vector3> CalculateSkillRange(Vector3 currentPos, Vector3 direction, int step)
    {
        range.Clear();

        var minRow = Mathf.RoundToInt(currentPos.z - step);
        var maxRow = Mathf.RoundToInt(currentPos.z + step);
        var minCol = Mathf.RoundToInt(currentPos.x - step);
        var maxCol = Mathf.RoundToInt(currentPos.x + step);

        for (int i = minRow; i <= maxRow; i++)
        {
            for (int j = minCol; j <= maxCol; j++)
            {
                range.Add(new Vector3(j,0f,i));
            }
        }

        return range;
    }
}
