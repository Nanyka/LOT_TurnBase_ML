using System;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    [Serializable]
    public class EcoBoss
    {
        public int expCondition;
        public string ecoBossName;
        public Vector3 spawnPos;
    }

    public class EcoBossSpawner : MonoBehaviour
    {
        [SerializeField] private List<EcoBoss> _ecoBosses;
        private EnvironmentManager _environmentManager;

        private void Start()
        {
            GameFlowManager.Instance.OnStartGame.AddListener(Init);
        }

        private void Init(long arg0)
        {
            _environmentManager = GameFlowManager.Instance.GetEnvManager();
            _environmentManager.OnChangeFaction.AddListener(MakeSpawningDecision);
        }

        private void MakeSpawningDecision()
        {
            for (int i = 0; i < _ecoBosses.Count; i++)
            {
                var ecoBoss = _ecoBosses[i];
                if (ecoBoss.expCondition <= SavingSystemManager.Instance.GetPlayerExp() &&
                    i == SavingSystemManager.Instance.GetGameProcess().ecoBoss)
                {
                    SavingSystemManager.Instance.OnSpawnMovableEntity(ecoBoss.ecoBossName,ecoBoss.spawnPos);
                    SavingSystemManager.Instance.SaveEcoBoss(i+1);
                }
            }
        }
    }
}