using System.Collections.Generic;
using UnityEngine;

public class PerpendicularWipe : SkillRange
{
    private List<Vector3> range = new();

    public Vector3 InvokeAt(Vector3 currentPos, Vector3 direction)
    {
        return currentPos;
    }

    public IEnumerable<Vector3> CalculateSkillRange(Vector3 currentPos, Vector3 direction, int step)
    {
        range.Clear();
        
        Vector2 currPos2D = new Vector2(currentPos.x, currentPos.z);
        direction *= -1f;
        Vector2 dir2D = new Vector2(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.z));
        Vector2 target = currPos2D + dir2D * step;
        if (Mathf.Abs(dir2D.x - dir2D.y) < Mathf.Epsilon)
        {
            range.Add(new Vector3(currentPos.x + direction.x, currentPos.y, currentPos.z));
            range.Add(new Vector3(currentPos.x, currentPos.y, currentPos.z + direction.z));
        }
        else
        {
            Vector2 perpendicularVec = Vector2.Perpendicular(target - currPos2D);
            Vector3 range1 = new Vector3(target.x + perpendicularVec.x, currentPos.y, target.y + perpendicularVec.y);
            Vector3 range2 = new Vector3(target.x - perpendicularVec.x, currentPos.y, target.y - perpendicularVec.y);
            
            range.Add(range1);
            range.Add(range2);
        }
        
        range.Add(new Vector3(target.x, currentPos.y, target.y));

        return range;
    }
}