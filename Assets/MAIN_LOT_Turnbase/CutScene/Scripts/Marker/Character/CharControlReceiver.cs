using System;
using UnityEngine;
using UnityEngine.Playables;

public interface IOnTrackController
{
    public void SetActive(bool isActive);
    public void Spawn();
    public void ActionOne();
    public void ActionTwo();
    public void ActionThree();
}

public class CharControlReceiver : MonoBehaviour, INotificationReceiver
{
    private IOnTrackController m_Controller;
    
    private void Start()
    {
        m_Controller = GetComponent<IOnTrackController>();
    }

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (notification != null)
        {
            var charMaker = notification as CharControlMarker;

            if (charMaker == null) return;
            
            // Set object as active state
            m_Controller.SetActive(charMaker.isActive);

            // Ask spawn action from the character
            if (charMaker.isSpawner)
                m_Controller.Spawn();
            
            // Ask for action one
            if (charMaker.isActionOne)
                m_Controller.ActionOne();
            
            // Ask for action two
            if (charMaker.isActionTwo)
                m_Controller.ActionTwo();
            
            // Ask for action three
            if (charMaker.isActionThree)
                m_Controller.ActionThree();
        }
    }
}