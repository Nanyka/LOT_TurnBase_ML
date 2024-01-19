using System;
using System.Collections;
using UnityEngine;

namespace JumpeeIsland
{
    // Transform means destroy this character and spawn a new one that may be the next level of this character
    // Step1: Ask for resource collecting periodically
    // Step2: Collect a certain amount of the resource
    // Step3: Check if full of resource to transform or not

    public class AoeCharacterTransformComp : MonoBehaviour, IZombieTransform
    {
        [SerializeField] private GameObject m_Brain;
        [SerializeField] private string _seekResourceState;
        [SerializeField] private CreatureData _transformerData;

        private IGetEntityData<CreatureStats> _entityData;
        private IBrain _brain;
        [SerializeField] private int _curStorage;
        [SerializeField] private bool _askForResource;

        private void Awake()
        {
            _entityData = GetComponent<IGetEntityData<CreatureStats>>();
            _brain = m_Brain.GetComponent<IBrain>();
        }

        private void Start()
        {
            InvokeRepeating(nameof(AskForResource),3f,3f);
        }

        private void AskForResource()
        {
            // if (_askForResource)
            //     return;

            _askForResource = true;
            _brain.GetBeliefStates().AddState(_seekResourceState);
        }

        public void StockResource(int amount)
        {
            _curStorage += amount;
            _askForResource = false;

            if (_curStorage >= _entityData.GetStats().CostToLevelUp)
                ExecuteTransform();
        }

        private void ExecuteTransform()
        {
            // Call to HpComp to destroy this character after a certain amount of second
            // Spawn the new character
            
            _brain.GetBeliefStates().RemoveState(_seekResourceState);
            GetComponent<IHealthComp>().HideTheEntity();
            StartCoroutine(WaitForSpawn());
        }

        private IEnumerator WaitForSpawn()
        {
            yield return new WaitForSeconds(2f);
            
            _transformerData.Position = transform.position;
            SavingSystemManager.Instance.OnSpawnMovableEntity(_transformerData);
        }
    }

    public interface IZombieTransform
    {
        public void StockResource(int amount);
    }
}