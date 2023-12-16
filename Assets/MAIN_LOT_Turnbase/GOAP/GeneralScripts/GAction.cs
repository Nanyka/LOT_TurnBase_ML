using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GOAP
{
    public abstract class GAction : MonoBehaviour
    {
        public string ActionName = "Action";
        public float Cost = 1.0f;
        [HideInInspector] public GameObject Target;
        [HideInInspector] public string TargetTag;
        public float Duration;
        
        [Tooltip("If it is for an in-place action, turn off IsChasePosition")]
        public bool IsAutoComplete;
        [Tooltip("Chase a specific position instead of gameObject")]
        [NonSerialized] public bool IsChasePosition;
        [Tooltip("Action finish right at the time motion stop")]
        public bool IsWaitAndStop;
        
        public WorldState[] PreConditions;
        public WorldState[] AfterEffects;
        [HideInInspector] public NavMeshAgent mNavMeshAgent;
        public Dictionary<string, int> DicPreConditions;
        public Dictionary<string, int> DicAfterEffects;
        private WorldStates AgentBeliefs;

        protected GAgent m_GAgent;
        // public GInventory Inventory;
        // public WorldStates Beliefs;

        public bool running = false;

        public GAction()
        {
            DicPreConditions = new Dictionary<string, int>();
            DicAfterEffects = new Dictionary<string, int>();
        }

        private void Awake()
        {
            mNavMeshAgent = GetComponent<NavMeshAgent>();

            if (PreConditions != null)
            {
                foreach (var preCondition in PreConditions)
                {
                    DicPreConditions.Add(preCondition.key,preCondition.value);
                }   
            }
            
            if (AfterEffects != null)
            {
                foreach (var afterEffect in AfterEffects)
                {
                    DicAfterEffects.Add(afterEffect.key,afterEffect.value);
                }   
            }

            m_GAgent = GetComponent<GAgent>();
            // Inventory = GetComponent<GAgent>().Inventory;
            // Beliefs = GetComponent<GAgent>().Beliefs;
        }

        public bool IsAchievable()
        {
            return true;
        }

        public bool IsAchievableGiven(Dictionary<string,int> conditions)
        {
            foreach (KeyValuePair<string,int> p in DicPreConditions)
            {
                if (!conditions.ContainsKey(p.Key))
                    return false;
            }

            return true;
        }

        // conduct some neccessary set up before this action is conducted
        public abstract bool PrePerform();
        public abstract bool PostPerform();
    }
}
