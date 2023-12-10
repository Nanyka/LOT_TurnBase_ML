using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class EnvObjectCreator : MonoBehaviour, IEnvironmentCreator
    {
        [SerializeField] private List<Vector3> _stones;

        public void CreateEnvObjects()
        {
            // for (int i = 0; i < _stoneAmount; i++)
            //     SavingSystemManager.Instance.OnSpawnResource("Tree1", GameFlowManager.Instance.GetEnvManager().GetRandomAvailableTile());

            foreach (var stone in _stones)
            {
                SavingSystemManager.Instance.OnSpawnResource("Tree1", stone);
            }
        }
    }

    public interface IEnvironmentCreator
    {
        public void CreateEnvObjects();
    }
}