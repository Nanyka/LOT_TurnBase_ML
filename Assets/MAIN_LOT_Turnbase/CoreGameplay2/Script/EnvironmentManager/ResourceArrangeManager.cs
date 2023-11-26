using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class ResourceArrangeManager : MonoBehaviour
    {
        [SerializeField] private int _stoneAmount;

        private void Start()
        {
            GameFlowManager.Instance.OnStartGame.AddListener(Init);
        }

        private void Init(long arg0)
        {
            for (int i = 0; i < _stoneAmount; i++)
            {
                SavingSystemManager.Instance.OnSpawnResource("Tree1",
                    GameFlowManager.Instance.GetEnvManager().GetRandomAvailableTile());
            }
        }
    }
}