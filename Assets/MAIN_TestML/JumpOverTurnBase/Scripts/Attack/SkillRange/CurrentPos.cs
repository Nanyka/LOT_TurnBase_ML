using System.Collections.Generic;
using UnityEngine;

public class CurrentPos : SkillRange
{
    public Vector3 InvokeAt(Vector3 currentPos, Vector3 direction)
    {
        return currentPos;
    }

    public IEnumerable<Vector3> CalculateSkillRange(Vector3 currentPos, Vector3 direction, int step)
    {
        return new []{currentPos};
    }
}