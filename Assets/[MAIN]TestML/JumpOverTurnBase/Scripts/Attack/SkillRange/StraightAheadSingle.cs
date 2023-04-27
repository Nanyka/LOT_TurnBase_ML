using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightAheadSingle : SkillRange
{
    private List<Vector3> range = new();

    public Vector3 InvokeAt(Vector3 currentPos, Vector3 direction)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable<Vector3> CalculateSkillRange(Vector3 currentPos, Vector3 direction, int step)
    {
        range.Clear();
        direction = new Vector3(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y), direction.z);
        Vector3 target = currentPos - direction * step;
        range.Add(target);
        return range;
    }
}
