using System.Collections;
using System.Collections.Generic;
using JumpeeIsland;
using UnityEngine;

namespace GOAP
{
    public class WorldTouchPoint : MonoBehaviour, ICheckableObject
    {
        [SerializeField] private string m_WorldState;
        [SerializeField] private bool isCheckable;

        private void OnEnable()
        {
            Invoke(nameof(Init),1f);
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
            if (GWorld.Instance != null)
                GWorld.Instance.GetWorld().ModifyState(m_WorldState,-1);
        }

        public bool IsCheckable()
        {
            return isCheckable;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public void ReduceCheckableAmount(int amount)
        {
            Debug.Log($"Reduce tower hp an amount: {amount}");
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}
