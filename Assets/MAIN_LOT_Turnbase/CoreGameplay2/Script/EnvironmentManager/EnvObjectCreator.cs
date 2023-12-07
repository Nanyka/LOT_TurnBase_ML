using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class EnvObjectCreator : MonoBehaviour, IEnvironmentCreator
    {
        [SerializeField] private int _stoneAmount;

        public void CreateEnvObjects()
        {
            for (int i = 0; i < _stoneAmount; i++)
                SavingSystemManager.Instance.OnSpawnResource("Tree1", GameFlowManager.Instance.GetEnvManager().GetRandomAvailableTile());
        }
    }

    public interface IEnvironmentCreator
    {
        public void CreateEnvObjects();
    }
}