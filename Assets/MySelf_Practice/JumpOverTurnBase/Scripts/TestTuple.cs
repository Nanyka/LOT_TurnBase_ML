using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTuple : MonoBehaviour
{
    private void Start()
    {
        var tupleReturn = CheckJump();
        Debug.Log($"Get the tuple with item1: {tupleReturn.movePoint} and item2: {tupleReturn.jumpStep}");
    }

    private (Vector3 movePoint, int jumpStep) CheckJump()
    {
        return (Vector3.forward, 2);
    }
}
