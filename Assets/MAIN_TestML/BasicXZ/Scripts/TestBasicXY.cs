using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestBasicXY : MonoBehaviour
{
    [SerializeField] private Collider _platformColider;
    
    private int _platformMaxCol;
    private int _platformMaxRow;
    private Dictionary<int, Vector3> _platformDic = new();

    private void Start()
    {
        var platformSize = _platformColider.bounds.size;
        platformSize = new Vector3(Mathf.RoundToInt(platformSize.x), Mathf.RoundToInt(platformSize.y),
            Mathf.RoundToInt(platformSize.z));
        _platformMaxCol = Mathf.RoundToInt((platformSize.x - 1) / 2);
        _platformMaxRow = Mathf.RoundToInt((platformSize.z - 1) / 2);
        
        SetUpPlatform();

        foreach (var pos in _platformDic)
        {
            Debug.Log($"Item {pos.Key} has value {pos.Value}");
        }
    }

    private void SetUpPlatform()
    {
        var platformSize = _platformColider.bounds.size;
        _platformMaxCol = Mathf.RoundToInt((platformSize.x - 1) / 2);
        _platformMaxRow = Mathf.RoundToInt((platformSize.z - 1) / 2);

        _platformDic.Clear();
        int index = 0;
        foreach (var zAxis in Enumerable.Range(-_platformMaxRow, Mathf.RoundToInt(platformSize.z)))
        {
            foreach (var xAxis in Enumerable.Range(-_platformMaxCol, Mathf.RoundToInt(platformSize.x)))
            {
                _platformDic.Add(index++,new Vector3(xAxis,zAxis));
            }
        }
    }
}