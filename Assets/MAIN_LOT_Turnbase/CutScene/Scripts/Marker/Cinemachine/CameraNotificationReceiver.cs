using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class CameraNotificationReceiver : MonoBehaviour, INotificationReceiver
{
    private CinemachineVirtualCamera m_Camera;

    private void Start()
    {
        m_Camera = GetComponent<CinemachineVirtualCamera>();
    }

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (notification != null)
        {
            double time = origin.IsValid() ? origin.GetTime() : 0.0;
            var cameraMarker = notification as CameraMarker;
            m_Camera.m_Lens.OrthographicSize = cameraMarker.lenSize;
        }
    }
}
