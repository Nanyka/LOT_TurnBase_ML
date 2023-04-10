using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCircle : MonoBehaviour
{
    [SerializeField] private Projector m_Projector;
    [SerializeField] private Collider m_Collider;

    private Transform m_Transform;
    private int _direction;

    private void Start()
    {
        m_Transform = transform;
    }

    public void SwitchProjector(Vector3 focusPos, int currDir)
    {
        m_Transform.position = focusPos;
        _direction = currDir;
        m_Projector.enabled = true;
        m_Collider.enabled = true;
    }

    public void SwitchProjector(bool turnOn)
    {
        m_Projector.enabled = turnOn;
        m_Collider.enabled = turnOn;
    }

    public int GetDirection()
    {
        return _direction;
    }
}
