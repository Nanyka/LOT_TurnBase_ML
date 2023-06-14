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
        public GameObject Target;
        public string TargetTag;
        public float Duration;
        
        public WorldState[] PreConditions;
        public WorldState[] AfterEffects;
        [HideInInspector] public NavMeshAgent Agent;
        public Dictionary<string, int> DicPreConditions;
        public Dictionary<string, int> DicAfterEffects;
        private WorldStates AgentBeliefs;

        public GInventory Inventory;
        public WorldStates Beliefs;

        public bool running = false;

        public GAction()
        {
            DicPreConditions = new Dictionary<string, int>();
            DicAfterEffects = new Dictionary<string, int>();
        }

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();

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

            Inventory = GetComponent<GAgent>().Inventory;
            Beliefs = GetComponent<GAgent>().Beliefs;
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
