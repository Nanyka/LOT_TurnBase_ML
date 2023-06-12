using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontArea : SkillRange
{
    private List<Vector3> range = new();

    public Vector3 InvokeAt(Vector3 currentPos, Vector3 direction)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Vector3> CalculateSkillRange(Vector3 currentPos, Vector3 direction, int step)
    {
        range.Clear();
        direction = new Vector3(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y), direction.z);
        Vector2 target = currentPos - direction * step;
        Vector2 currPos2D = currentPos;
        if (Math.Abs(direction.x - direction.y) < Mathf.Epsilon)
        {
            range.Add(new Vector3(currentPos.x - direction.x, currentPos.y, currentPos.z));
            range.Add(new Vector3(currentPos.x, currentPos.y - direction.y, currentPos.z));
        }
        else
        {
            Vector2 perpendicularVec = Vector2.Perpendicular(target - currPos2D);

            range.Add(target + perpendicularVec);
            range.Add(target - perpendicularVec);
        }
        range.Add(target);
        
        return range;
    }
}