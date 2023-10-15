using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    [RequireComponent(typeof(Entity))]
    public class SpawnEntityComp : MonoBehaviour
    {
        [SerializeField] private string m_PurchaseId;

        private Entity m_Entity;
        
        private void Start()
        {
            m_Entity = GetComponent<Entity>();
            m_Entity.OnUnitDie.AddListener(SpawnEntity);
            
        }

        private void SpawnEntity(Entity killByEntity)
        {
            SavingSystemManager.Instance.OnSpawnMovableEntity(m_PurchaseId, m_Entity.GetData().Position);
        }
    }
}
