using System.Collections.Generic;
using UnityEngine;

public class Circle : SkillRange
{
    private List<Vector3> range = new();

    public Vector3 InvokeAt(Vector3 currentPos, Vector3 direction)
    {
        return currentPos;
    }

    public IEnumerable<Vector3> CalculateSkillRange(Vector3 currentPos, Vector3 direction, int step)
    {
        range.Clear();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;
                
                Vector3 addPos = new Vector3(currentPos.x + i * step, currentPos.y, currentPos.z + j * step);
                range.Add(addPos);
            }
        }

        return range;
    }
}