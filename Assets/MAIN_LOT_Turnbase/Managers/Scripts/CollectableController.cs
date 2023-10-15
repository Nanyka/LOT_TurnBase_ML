using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class CollectableController : MonoBehaviour
    {
        private List<CollectableInGame> m_CollectableInGame = new();
        private EnvironmentManager m_Environment;

        public void AddCollectableToList(CollectableInGame collectableInGame)
        {
            m_CollectableInGame.Add(collectableInGame);
        }

        public void Init()
        {
            m_Environment = GameFlowManager.Instance.GetEnvManager();
            m_Environment.OnChangeFaction.AddListener(DurationDeduct);
        }

        private void DurationDeduct()
        {
            if (m_Environment.GetCurrFaction() != FactionType.Player)
                return;

            foreach (var collectableInGame in m_CollectableInGame)
                collectableInGame.DurationDeduct();
        }

        public void RemoveCollectable(CollectableInGame collectbInGame)
        {
            m_CollectableInGame.Remove(collectbInGame);
        }

        public CollectableInGame GetCollectableByPos(Vector3 atPos)
        {
            return m_CollectableInGame.Find(t => Vector3.Distance(t.transform.position, atPos) < 0.1f);
        }
    }
}