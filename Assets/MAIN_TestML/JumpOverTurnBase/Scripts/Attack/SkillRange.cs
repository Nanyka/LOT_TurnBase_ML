using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SkillRange
{
    public Vector3 InvokeAt(Vector3 currentPos, Vector3 direction);
    
    public IEnumerable<Vector3> CalculateSkillRange(Vector3 currentPos, Vector3 direction, int step);
}
