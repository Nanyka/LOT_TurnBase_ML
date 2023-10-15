using UnityEngine;

namespace JumpeeIsland
{
    public class PlayerNpcController : NpcFactionController
    {
        private bool _isInitiated;

        public override void Init()
        {
            m_Environment = GameFlowManager.Instance.GetEnvManager();
            m_NpcActionInferer = GetComponent<NPCActionInferer>();
            m_NpcActionInferer.Init();
            _camera = Camera.main;
            _isInitiated = true;
        }

        public override void Update()
        {
            if (_isInitiated == false)
                return;
            
            base.Update();
        }

        public override void ToMyTurn()
        {
            // reset all agent's moving state
            foreach (var enemy in m_NpcUnits)
                enemy.NewTurnReset();

            KickOffNewTurn();
        }
    }
}