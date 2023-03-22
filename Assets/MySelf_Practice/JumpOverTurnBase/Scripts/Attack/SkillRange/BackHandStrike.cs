using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackHandStrike : SkillRange
{
    private List<Vector3> range = new();

    public Vector3 InvokeAt(Vector3 currentPos, Vector3 direction)
    {
        return currentPos;
    }

    public IEnumerable<Vector3> CalculateSkillRange(Vector3 currentPos, Vector3 direction, int step)
    {
        range.Clear();
        CheckPosition(currentPos, -1f * direction, 0, step);

        return range;
    }

    private void CheckPosition(Vector3 currPos, Vector3 direction, int accumulateStep, int maxStep)
    {
        if (accumulateStep >= maxStep)
            return;

        currPos += direction;
        range.Add(currPos);
        accumulateStep++;
        CheckPosition(currPos, direction, accumulateStep, maxStep);
    }
}