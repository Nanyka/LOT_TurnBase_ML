using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private Collider _collider;
    private int _occupyHorizontal;
    private int _occupyVertical;
    
    private void CalculateNumberOccupy()
    {
        Vector3 size = _collider.bounds.size;
        _occupyHorizontal = Mathf.RoundToInt(size.x);
        _occupyVertical = Mathf.RoundToInt(size.z);
    }

    public IEnumerable<Vector3> GetOccupyRange()
    {
        CalculateNumberOccupy();
        
        List<Vector3> occupyRange = new List<Vector3>();
        var currentPos = transform.position;
        
        for (int i = 0; i < _occupyHorizontal; i++)
            for (int j = 0; j < _occupyVertical; j++)
                occupyRange.Add(new Vector3(currentPos.x + i, currentPos.y, currentPos.z + j));
        return occupyRange;
    }
}