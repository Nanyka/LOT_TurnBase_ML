using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class ResearchLoader : MonoBehaviour, IResearchTopicSupervisor
    {
        private IInventoryDeliver m_Inventory;
        private List<Research> m_Researches = new();

        private void Start()
        {
            m_Inventory = GetComponent<IInventoryDeliver>();
            GameFlowManager.Instance.OnStartGame.AddListener(LoadResearchTopics);
        }

        private void LoadResearchTopics(long arg0)
        {
            var inventories = m_Inventory.GetInventoriesByType(InventoryType.Creature);
            
            foreach (var item in inventories)
            {
                var troopLevel = SavingSystemManager.Instance.GetInventoryLevel(item.inventoryName);

                for (int i = 1; i <= troopLevel+1; i++)
                {
                    var newTopic = new Research()
                    {
                        ResearchType = ResearchType.TROOP_TRANSFORM,
                        TroopType = TroopType.NONE,
                        TroopStats = TroopStats.NONE,
                        Target = item.inventoryName,
                        Magnitude = i,
                        Difficulty = 0,
                        Explaination = $"{item.inventoryName} learn new skill"
                    };
                    m_Researches.Add(newTopic);
                }
            }
        }

        public IEnumerable<Research> GetTopics()
        {
            return m_Researches;
        }
    }

    public interface IResearchTopicSupervisor
    {
        public IEnumerable<Research> GetTopics();
    }
}