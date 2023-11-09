using System;
using UnityEngine;

namespace GOAP
{
    public class WorldTouchPoint : MonoBehaviour
    {
        [SerializeField] private string m_WorldState;
        
        private void OnEnable()
        {
            Init();
        }

        private void OnDisable()
        {
            Destroyed();
        }

        private void Init()
        {
            GWorld.Instance.GetWorld().ModifyState(m_WorldState,1);
        }

        private void Destroyed()
        {
            GWorld.Instance.GetWorld().ModifyState(m_WorldState,-1);
        }
    }
}