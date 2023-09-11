using System.Collections.Generic;
using UnityEngine;

public class TShapeFront : SkillRange
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
        Vector2 dir2D = new Vector2(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.z));
        for (int i = 1; i <= step; i++)
        {
            Vector2 target = currPos2D + dir2D * i;
            Vector2 perpendicularVec = Vector2.Perpendicular(target - currPos2D);
            range.Add(new Vector3(currPos2D.x + perpendicularVec.x*i, currentPos.y, currPos2D.y + perpendicularVec.y*i));
            range.Add(new Vector3(currPos2D.x - perpendicularVec.x*i, currentPos.y, currPos2D.y - perpendicularVec.y*i));
            range.Add(new Vector3(target.x, currentPos.y, target.y));
        }

        return range;
    }
}