using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class FactoryComp : MonoBehaviour, IInputExecutor, IStoreResource, IResearchDeliver
    {
        [SerializeField] private Vector3 assemblyPoint;
        [SerializeField] private TroopType troopType;
        [SerializeField] private string troopName;
        [SerializeField] private int troopCount;
        [SerializeField] private int costPerTroop;
        [SerializeField] private Transform testAssemblyPoint;

        private int _curStorage;
        private BuildingEntity m_Building;
        private Research m_Research;
        private bool _isOnHolding;

        private void Start()
        {
            m_Building = GetComponent<BuildingEntity>();
        }

        public void OnClick()
        {
            Debug.Log($"Click on {name} to change priority");
        }

        public void OnHoldEnter()
        {
            _isOnHolding = true;
        }

        public void OnHolding(Vector3 position)
        {
            assemblyPoint = position;
            testAssemblyPoint.position = assemblyPoint;
        }

        public async void OnHoldCanCel()
        {
            if (_isOnHolding == false)
                return;

            _isOnHolding = false;
            if (_curStorage < costPerTroop)
                return;

            if (_curStorage >= costPerTroop * troopCount)
                m_Building.GetWorldStateChanger()
                    .ChangeState(1); // restart the factory after release a part of its storage

            var spawnAmount = Mathf.FloorToInt(_curStorage * 1f / costPerTroop);
            for (int i = 0; i < spawnAmount; i++)
            {
                var troop = await SavingSystemManager.Instance.OnTrainACreature(troopName, transform.position, false);
                if (troop == null)
                    continue;

                if (troop.TryGetComponent(out ITroopAssembly character))
                {
                    Debug.Log("Set assembly point");
                    character.SetAssemblyPoint(assemblyPoint);
                }

                if (troop.TryGetComponent(out ISpecialAttackReceiver attackReceiver))
                {
                    // Check if any storage research in the stock
                    // Plug the skill in the entity

                    if (m_Research != null)
                        attackReceiver.EnablePowerBar(m_Research.Magnitude);
                }
            }

            _curStorage -= spawnAmount * costPerTroop;
        }

        public void OnDoubleTaps()
        {
            Debug.Log($"Double taps on {name}");
        }

        public bool IsFullStock()
        {
            return _curStorage >= costPerTroop * troopCount;
        }

        public void StoreResource(int amount)
        {
            _curStorage += amount;
            if (_curStorage >= costPerTroop * troopCount)
            {
                _curStorage = costPerTroop * troopCount;
                m_Building.GetWorldStateChanger().ChangeState(-1); // decrease one unit of available factory
            }
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public bool CheckTarget(string targetName)
        {
            return troopName.Equals(targetName);
        }

        public bool CheckTroopType(TroopType checkType)
        {
            return checkType == TroopType.NONE || checkType == troopType;
        }

        public void LoadResearch(Research research)
        {
            m_Research = research;
        }
    }

    public interface IStoreResource
    {
        public bool IsFullStock();
        public void StoreResource(int amount);
        public GameObject GetGameObject();
    }

    public interface IResearchDeliver
    {
        public bool CheckTarget(string targetName);
        public bool CheckTroopType(TroopType checkType);
        public void LoadResearch(Research research);
    }
}