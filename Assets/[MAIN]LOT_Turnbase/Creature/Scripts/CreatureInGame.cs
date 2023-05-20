using System.Collections;
using System.Collections.Generic;
using LOT_Turnbase;
using UnityEngine;

namespace LOT_Turnbase
{
    public class CreatureInGame : MonoBehaviour
    {
        [SerializeField] private CreatureEntity m_Entity;
        
        public void Init(CreatureData creatureData)
        {
            m_Entity.Init(creatureData);
            // Debug.Log("Initiate this resource at " + m_ResourceData.Position);
        }
    }
}
