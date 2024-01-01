using System.Linq;
using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class ChaseBoss : GAction, IProcessUpdate
    {
        [SerializeField] private CharacterEntity m_Entity;

        public override bool PrePerform()
        {
            if (m_GAgent.Inventory.IsEmpty())
            {
                GameObject currentPoint = null;
                var distanceToTarget = float.PositiveInfinity;
                var monsters = SavingSystemManager.Instance.GetMonsterController().GetMonsters();
                
                foreach (var monster in monsters)
                {
                    var curDis = Vector3.Distance(transform.position, monster.transform.position);
                    if (curDis < distanceToTarget)
                    {
                        distanceToTarget = curDis;
                        currentPoint = monster;
                    }
                }

                if (currentPoint != null)
                    m_GAgent.Inventory.AddItem(currentPoint);
            }

            if (m_GAgent.Inventory.items.Count == 0)
                return false;
            
            Target = m_GAgent.Inventory.items[0];
            m_GAgent.SetIProcessUpdate(this);

            return true;
        }

        public override bool PostPerform()
        {
            return true;
        }

        public void StartProcess(Transform myTransform, Vector3 targetPos)
        {
            if (Vector3.Distance(myTransform.position, targetPos) < m_Entity.GetStopDistance())
                m_GAgent.FinishFromOutside();
            else
                m_Entity.MoveTowards(targetPos, this);
        }

        public void StopProcess()
        {
            m_GAgent.FinishFromOutside();
        }
    }
}